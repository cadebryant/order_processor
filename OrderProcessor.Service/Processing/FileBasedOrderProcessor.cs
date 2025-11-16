using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Pricing;
using OrderProcessor.Service.Sinks;

namespace OrderProcessor.Service.Processing
{
    public class FileBasedOrderProcessor : IOrderProcessor
    {
        private readonly IValidator<ProcessRequest> _validator;
        private readonly IClock _clock;
        private readonly IReportSink _sink;
        private readonly PricingConfig _config;
        private readonly IPricingEngine _engine;
        private readonly IOrderParser _parser;
        private readonly IReportFormatter _formatter;
        private readonly Serilog.ILogger _logger;

        public FileBasedOrderProcessor(
            IValidator<ProcessRequest> validator,
            IClock clock,
            IReportSink sink,
            IOptions<PricingConfig> config,
            IPricingEngine engine,
            IOrderParser parser,
            IReportFormatter formatter,
            Serilog.ILogger logger)
        {
            _validator = validator;
            _clock = clock;
            _sink = sink;
            _config = config.Value;
            _engine = engine;
            _parser = parser;
            _formatter = formatter;
            _logger = logger;
        }

        public async Task<IResult> GetOrderReportAsync(string id, CancellationToken ct)
        {
            var path = $"Output/report_{id}.txt";
            if (!await _sink.ExistsAsync(path, ct))
                return Results.NotFound();

            var lines = await _sink.ReadAllLinesAsync(path, ct);
            return Results.Text(string.Join(Environment.NewLine, lines), "text/plain");
        }

        public async Task<Results<Ok<ProcessResponse>, ValidationProblem>> ProcessOrdersAsync(HttpRequest processOrdersRequest, CancellationToken ct)
        {
            _logger.Information("Begin processing request");

            if (processOrdersRequest is null)
            {
                _logger.Warning("Received null request");
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { string.Empty, OrderConstants.ValidationMessages.RequestCannotBeNull }
                });
            }

            string raw;
            if (processOrdersRequest.ContentType != null && processOrdersRequest.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                var body = await processOrdersRequest.ReadFromJsonAsync<ProcessRequest>(cancellationToken: ct);
                if (body is null)
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { string.Empty, OrderConstants.ValidationMessages.InvalidJson }
                    });
                var vdr = await _validator.ValidateAsync(body, ct);
                if (!vdr.IsValid)
                    return TypedResults.ValidationProblem(vdr.ToDictionary());
                raw = body.Csv!;
            }
            else
            {
                using var reader = new StreamReader(processOrdersRequest.Body);
                raw = await reader.ReadToEndAsync(ct);
                var vdr = await _validator.ValidateAsync(new ProcessRequest { Csv = raw }, ct);
                if (!vdr.IsValid)
                    return TypedResults.ValidationProblem(vdr.ToDictionary());
            }

            var lines = raw.SplitLines();
            if (lines.FirstOrDefault().LooksLikeHeader())
                lines = [.. lines.Skip(1)];

            var rows = new List<string> { _formatter.Header, new('-', 95) };
            var count = 0;
            decimal gross = 0m, revenue = 0m;

            foreach (var l in lines.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                if (!_parser.TryParse(l, out var order))
                    continue;
                count++;
                gross += order.Amount;
                var result = _engine.Price(order);
                revenue += result.Net;
                rows.Add(_formatter.FormatRow(order, result));
            }

            rows.AddRange(_formatter.FormatSummary(count, (double)gross, (double)revenue));
            var id = Guid.NewGuid().ToString("n");
            await _sink.WriteAllLinesAsync($"Output/report_{id}.txt", rows, ct);

            _logger.Information("Finished processing request; wrote report {ReportId}", id);

            return TypedResults.Ok(new ProcessResponse
            {
                TotalOrders = count,
                Gross = Math.Round(gross, 2),
                Revenue = Math.Round(revenue, 2)
            });
        }
    }
}
