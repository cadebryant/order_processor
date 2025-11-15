using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Pricing;
using OrderProcessor.Service.Sinks;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<PricingConfig>(builder.Configuration.GetSection("Pricing"));
builder.Services.Configure<FallbackConfig>(builder.Configuration.GetSection("Fallback"));
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IOrderParser, NaiveCsvOrderParser>();
builder.Services.AddSingleton<IPricingEngine, PricingEngine>();
builder.Services.AddSingleton<IReportFormatter, TableReportFormatter>();
builder.Services.AddSingleton<ICustomerCache, InMemoryCustomerCache>();
builder.Services.AddSingleton<IReportSink, FileReportSink>();
builder.Services.AddSingleton<ILineSource>(sp =>
    new FileOrFallbackLineSource("orders.csv", sp.GetRequiredService<IOptions<FallbackConfig>>()));
// register validators from the assembly that contains the domain types (where ProcessRequest lives)
builder.Services.AddValidatorsFromAssemblyContaining<ProcessRequest>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrderProcessor API",
        Version = "v1",
        Description = "Minimal API for processing orders and retrieving reports"
    });

    // include XML comments if the XML file is generated
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation());

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(e =>
{
    e.Run(async context =>
    {
        var ex = context.Features.GetRequiredFeature<IExceptionHandlerFeature>().Error;
        var problem = Results.Problem(
            title: "Unhandled Exception",
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError);
        await problem.ExecuteAsync(context);
    });
});

// enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderProcessor API v1"));
}
else
{
    // optional: expose swagger in non-dev too (adjust as desired)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderProcessor API v1");
    });
}

app.MapHealthChecks("/health");
app.MapPost("/process", async Task<Results<Ok<ProcessResponse>, ValidationProblem>> (
    HttpRequest req,
    [FromServices] IValidator<ProcessRequest> validator,
    [FromServices] IClock clock,
    [FromServices] IReportSink sink,
    [FromServices] IOptions<PricingConfig> config,
    [FromServices] IPricingEngine engine,
    [FromServices] IOrderParser parser,
    [FromServices] IReportFormatter formatter,
    CancellationToken ct) =>
{
    if (req is null)
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            { string.Empty, OrderConstants.ValidationMessages.RequestCannotBeNull }
        });
    }

    string raw;
    if (req.ContentType != null && req.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
    {
        var body = await req.ReadFromJsonAsync<ProcessRequest>(cancellationToken: ct);
        if (body is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { string.Empty, OrderConstants.ValidationMessages.InvalidJson }
            });
        var vdr = await validator.ValidateAsync(body, ct);
        if (!vdr.IsValid)
            return TypedResults.ValidationProblem(vdr.ToDictionary());
        raw = body.Csv!;
    }
    else
    {
        using var reader = new StreamReader(req.Body);
        raw = await reader.ReadToEndAsync(ct);
        var vdr = await validator.ValidateAsync(new ProcessRequest { Csv = raw }, ct);
        if (!vdr.IsValid)
            return TypedResults.ValidationProblem(vdr.ToDictionary());
    }

    var lines = raw.SplitLines();
    if (lines.FirstOrDefault().LooksLikeHeader())
        lines = [.. lines.Skip(1)];

    var rows = new List<string> { formatter.Header, new('-', 95) };
    var count = 0;
    decimal gross = 0m, revenue = 0m;

    foreach (var l in lines.Where(s => !string.IsNullOrWhiteSpace(s)))
    {
        if (!parser.TryParse(l, out var order))
            continue;
        count++;
        gross += order.Amount;
        var result = engine.Price(order);
        revenue += result.Net;
        rows.Add(formatter.FormatRow(order, result));
    }

    rows.AddRange(formatter.FormatSummary(count, (double)gross, (double)revenue));
    var id = Guid.NewGuid().ToString("n");
    await sink.WriteAllLinesAsync($"report_{id}.txt", rows, ct);

    return TypedResults.Ok(new ProcessResponse
    {
        TotalOrders = count,
        Gross = Math.Round(gross, 2),
        Revenue = Math.Round(revenue, 2)
    });
})
.Accepts<ProcessRequest>("application/json")
.Produces<ProcessResponse>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status400BadRequest);

app.MapGet("/report/{id}", async Task<IResult> (string id, IReportSink sink, CancellationToken ct) =>
{
    var path = $"report_{id}.txt";
    if (!await sink.ExistsAsync(path, ct))
        return Results.NotFound();

    var lines = await sink.ReadAllLinesAsync(path, ct);
    return Results.Text(string.Join(Environment.NewLine, lines), "text/plain");
});

app.Run();