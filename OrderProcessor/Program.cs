using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Pricing;
using OrderProcessor.Service.Processing;
using Serilog;
using System.Globalization;
using System.Text.Json;
using Processor = OrderProcessor.Service.Processing.OrderProcessor;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<PricingConfig>(builder.Configuration.GetSection("Pricing"));
builder.Services.Configure<FallbackConfig>(builder.Configuration.GetSection("Fallback"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IOrderParser, NaiveCsvOrderParser>();
builder.Services.AddSingleton<IPricingEngine, PricingEngine>();
builder.Services.AddSingleton<IReportFormatter, TableFormatter>();
builder.Services.AddSingleton<IOrderProcessor, Processor>();
builder.Services.AddSingleton<ICustomerCache, InMemoryCustomerCache>();
builder.Services.AddSingleton<ILineSource>(sp =>
    new FileOrFallbackLineSource("orders.csv", sp.GetRequiredService<IOptions<FallbackConfig>>()));

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

var app = builder.Build();
var config = app.Services.GetRequiredService<IOptions<FallbackConfig>>().Value;
Console.WriteLine(JsonSerializer.Serialize(config));