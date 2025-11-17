using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.Pricing;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Processing;
using OrderProcessor.Service.Sinks;
using OrderProcessor.Service.Validation;
using OrderProcessor.Service.IO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessorTests
{
    [TestClass]
    public sealed class OrderProcessorUnitTests
    {
        private readonly PricingConfig _pricingConfig = new()
        {
            FoodMultiplier = 1.1m,
            ElectronicsMultiplier = 1.2m,
            OtherMultiplier = 1.3m,
            NyTax = 0.08m,
            CaTax = 0.075m,
            DefaultTax = 0.05m
        };

        [TestMethod]
        public void PricingConfig_TypeMap_ReturnsOther_IfItemTypeNotFound()
        {
            // Arrange
            var unknownItemType = "Toys";
            var expectedMultiplier = _pricingConfig.GetPriceMultiplier(OrderConstants.Types.Other);

            // Act
            var actualMultiplier = _pricingConfig.GetPriceMultiplier(unknownItemType);

            // Assert
            Assert.AreEqual(expectedMultiplier, actualMultiplier);
        }

        [TestMethod]
        public void PricingConfig_TypeMap_ReturnsCorrectMultiplier_ForKnownItemTypes()
        {
            // Arrange
            var testCases = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                { OrderConstants.Types.Food, _pricingConfig.FoodMultiplier },
                { OrderConstants.Types.Electronics, _pricingConfig.ElectronicsMultiplier },
                { OrderConstants.Types.Other, _pricingConfig.OtherMultiplier }
            };

            // Act / Assert
            foreach (var testCase in testCases)
            {
                var actualMultiplier = _pricingConfig.GetPriceMultiplier(testCase.Key);
                Assert.AreEqual(testCase.Value, actualMultiplier, $"Failed for item type: {testCase.Key}");
            }
        }

        [TestMethod]
        public void NaiveCsvOrderParser_ParseLine_ParsesValidLineCorrectly()
        {
            // Arrange
            var clock = new TestClock(new DateTime(2025, 11, 10));
            var parser = new NaiveCsvOrderParser(clock);
            var line = "1,John Doe,Food,100.0,2025-11-10,North,CA";

            // Act
            var ok = parser.TryParse(line, out var order);

            // Assert
            Assert.IsTrue(ok);
            Assert.IsNotNull(order);
            Assert.AreEqual("1", order.Id);
            Assert.AreEqual("John Doe", order.Customer);
            Assert.AreEqual("Food", order.Type);
            Assert.AreEqual(100.0m, order.Amount);
            Assert.AreEqual(new DateTime(2025, 11, 10), order.Date);
            Assert.AreEqual("North", order.Region);
            Assert.AreEqual("CA", order.State);
        }

        [TestMethod]
        public void NaiveCsvOrderParser_ParseLine_FailsOnInvalidLine()
        {
            // Arrange
            var clock = new TestClock(new DateTime(2025, 11, 10));
            var parser = new NaiveCsvOrderParser(clock);
            var line = "";

            // Act & Assert
            Assert.IsFalse(parser.TryParse(line, out var _));
        }

        [TestMethod]
        public void PricingEngine_Price_ComputesNetAndTax()
        {
            // Arrange
            var options = Options.Create(_pricingConfig);
            var engine = new PricingEngine(options);

            var orderFood = new Order("1", "Alice", OrderConstants.Types.Food, 100.00m, DateTime.Today, "North", "CA");
            var orderElectronics = new Order("2", "Bob", OrderConstants.Types.Electronics, 200.00m, DateTime.Today, "South", "NY");

            // Act
            var resultFood = engine.Price(orderFood);
            var resultElectronics = engine.Price(orderElectronics);

            // Assert
            // Food: 100 * 1.1 = 110 -> CA tax 0.075 => net = 110 * 1.075 = 118.25
            Assert.AreEqual(118.25m, resultFood.Net);
            // Electronics: 200 * 1.2 = 240 -> NY tax 0.08 => net = 240 * 1.08 = 259.20
            Assert.AreEqual(259.20m, resultElectronics.Net);

            // Tax rates are rounded to 2 decimals by PricingEngine
            Assert.AreEqual(decimal.Round(_pricingConfig.GetStateTaxRate("CA"), 2), resultFood.TaxRate);
            Assert.AreEqual(decimal.Round(_pricingConfig.GetStateTaxRate("NY"), 2), resultElectronics.TaxRate);
        }

        [TestMethod]
        public void TableReportFormatter_FormatsRowAndSummary()
        {
            // Arrange
            var formatter = new TableReportFormatter();
            var order = new Order("1", "Alice", OrderConstants.Types.Food, 100.0m, new DateTime(2025, 11, 10), "North", "CA");
            var pricingResult = new PricingResult(118.25m, 0.08m);

            // Act
            var row = formatter.FormatRow(order, pricingResult);
            var summary = formatter.FormatSummary(1, 100.0, 118.25).ToArray();

            // Assert
            Assert.Contains("Alice", row);
            Assert.Contains("118.25", row);
            Assert.HasCount(5, summary);
        }

        [TestMethod]
        public async Task FileBasedOrderProcessor_ProcessOrders_WritesReportAndReturnsCorrectResponse()
        {
            // Arrange - end-to-end but in-memory sink
            var validator = new ProcessRequestValidator();
            var clock = new TestClock(new DateTime(2025, 11, 10));
            var formatter = new TableReportFormatter();
            var sink = new InMemoryReportSink();
            var parser = new NaiveCsvOrderParser(clock);
            var engine = new PricingEngine(Options.Create(_pricingConfig));
            var logger = new Serilog.LoggerConfiguration().CreateLogger();

            var processor = new FileBasedOrderProcessor(
                validator,
                clock,
                sink,
                Options.Create(_pricingConfig),
                engine,
                parser,
                formatter,
                logger);

            // CSV with header + two rows
            var csv = new StringBuilder();
            csv.AppendLine("id,customer,type,amount,date,region,state");
            csv.AppendLine("1,John Doe,Food,100.00,2025-11-10,North,CA");
            csv.AppendLine("2,Jane Smith,Electronics,200.00,2025-11-10,South,NY");

            // create HttpRequest with raw body (not JSON) so FileBasedOrderProcessor reads the stream
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
            context.Request.ContentType = "text/csv";

            // Act
            var res = await processor.ProcessOrdersAsync(context.Request, CancellationToken.None);

            // Assert: result is Results<Ok<ProcessResponse>, ValidationProblem>
            var ok = res?.Result as Ok<ProcessResponse>;
            Assert.IsNotNull(ok, "Expected Ok<ProcessResponse> result");
            var payload = ok!.Value;
            Assert.AreEqual(2, payload!.TotalOrders);
            Assert.AreEqual(300.00m, payload.Gross);
            Assert.AreEqual(377.45m, payload.Revenue); // 118.25 + 259.20 = 377.45

            // Verify sink wrote a report
            Assert.IsNotNull(sink.LastPath);
            Assert.StartsWith("Output/report_", sink.LastPath);
            Assert.IsGreaterThanOrEqualTo(3, sink.LastLines.Length); // header + rows + summary
            Assert.AreEqual(formatter.Header, sink.LastLines[0]);
        }

        [TestMethod]
        public async Task FileBasedOrderProcessor_GetOrderReport_ReturnsReportContent()
        {
            // Arrange
            var validator = new ProcessRequestValidator();
            var clock = new TestClock(new DateTime(2025, 11, 10));
            var formatter = new TableReportFormatter();
            var sink = new InMemoryReportSink();
            var parser = new NaiveCsvOrderParser(clock);
            var engine = new PricingEngine(Options.Create(_pricingConfig));
            var logger = new Serilog.LoggerConfiguration().CreateLogger();

            var processor = new FileBasedOrderProcessor(
                validator,
                clock,
                sink,
                Options.Create(_pricingConfig),
                engine,
                parser,
                formatter,
                logger);

            // Pre-populate sink with a known report id
            var id = "testreportid";
            var path = $"Output/report_{id}.txt";
            var content = new[] { formatter.Header, new string('-', 95), "Total Orders: 0" };
            await sink.WriteAllLinesAsync(path, content, CancellationToken.None);

            // Act
            var result = await processor.GetOrderReportAsync(id, CancellationToken.None);
            var httpContext = new DefaultHttpContext();
            var services = new ServiceCollection();
            services.AddLogging(); // registers ILoggerFactory and related services
            httpContext.RequestServices = services.BuildServiceProvider();
            httpContext.Response.Body = new MemoryStream();

            await result.ExecuteAsync(httpContext);
            httpContext.Response.Body.Position = 0;
            var text = new StreamReader(httpContext.Response.Body).ReadToEnd();

            // Assert: expect Results.Text
            var expected = string.Join(Environment.NewLine, content);
            Assert.AreEqual(expected, text);
            Assert.AreEqual("text/plain", httpContext.Response.ContentType);
        }

        // --- Helpers used in tests ---

        private sealed class TestClock : IClock
        {
            private readonly DateTime _today;
            public TestClock(DateTime today) => _today = today;
            public DateTime Today() => _today;
        }

        private sealed class InMemoryReportSink : IReportSink
        {
            private readonly Dictionary<string, string[]> _store = new(StringComparer.OrdinalIgnoreCase);
            public string[] LastLines { get; private set; } = [];
            public string? LastPath { get; private set; }

            public Task<bool> ExistsAsync(string path, CancellationToken ct) =>
                Task.FromResult(_store.ContainsKey(path));

            public Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct) =>
                Task.FromResult(_store.TryGetValue(path, out var lines) ? lines : Array.Empty<string>());

            public Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken ct)
            {
                var arr = lines.ToArray();
                _store[path] = arr;
                LastLines = arr;
                LastPath = path;
                return Task.CompletedTask;
            }
        }
    }
}
