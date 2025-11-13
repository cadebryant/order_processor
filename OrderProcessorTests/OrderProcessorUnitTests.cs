using OrderProcessor.Service.Config;
using OrderProcessor.Service.Parsing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Domain;
using Microsoft.Extensions.Caching.Memory;
using OrderProcessor.Service.Formatting;
using Processor = OrderProcessor.Service.Processing.OrderProcessor;
using OrderProcessor.Service.Pricing;

namespace OrderProcessorTests
{
    [TestClass]
    public sealed class OrderProcessorUnitTests
    {
        private readonly ILogger<PricingEngine> _pricingEngineLogger = NullLogger<PricingEngine>.Instance;
        private readonly ILogger<NaiveCsvOrderParser> _csvOrderLogger = NullLogger<NaiveCsvOrderParser>.Instance;
        private readonly ILogger<Processor> _orderProcesserLogger = NullLogger<Processor>.Instance;
        private readonly IOrderParser _parser = Substitute.For<IOrderParser>();
        private readonly IClock _clock = Substitute.For<IClock>();
        private readonly ILineSource _lineSource = Substitute.For<ILineSource>();

        [TestInitialize]
        public void TestInitialize()
        {
            _clock.Today().Returns(DateTime.Today);
        }
        //[TestMethod]
        //public void PricingConfig_TypeMap_ReturnsOther_IfItemTypeNotFound()
        //{
        //    // Arrange
        //    var unknownItemType = "Toys";
        //    var expectedMultiplier = PricingConfig.GetPriceMultiplier(PricingConfig.Types.Other);
        //    // Act
        //    var actualMultiplier = PricingConfig.GetPriceMultiplier(unknownItemType);
        //    // Assert
        //    Assert.AreEqual(expectedMultiplier, actualMultiplier);
        //}

        //[TestMethod]
        //public void PricingConfig_TypeMap_ReturnsCorrectMultiplier_ForKnownItemTypes()
        //{
        //    // Arrange
        //    var testCases = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        //    {
        //        { PricingConfig.Types.Food, PricingConfig.Types.FoodMultiplier },
        //        { PricingConfig.Types.Electronics, PricingConfig.Types.ElectronicsMultiplier },
        //        { PricingConfig.Types.Other, PricingConfig.Types.OtherMultiplier }
        //    };
        //    foreach (var testCase in testCases)
        //    {
        //        // Act
        //        var actualMultiplier = PricingConfig.GetPriceMultiplier(testCase.Key);
        //        // Assert
        //        Assert.AreEqual(testCase.Value, actualMultiplier, $"Failed for item type: {testCase.Key}");
        //    }
        //}

        //[TestMethod]
        //public void OrderProcessor_ProcessOrders_ParsesAndProcessesOrdersCorrectly()
        //{
        //    // Arrange
        //    _lineSource.GetLines().Returns(new List<string> { "1,John Doe,Food,100.0,2025-11-10,North,CA" });
        //    var pricingEngine = new PricingEngine(_pricingEngineLogger);
        //    var customerCache = new InMemoryCustomerCache(new MemoryCache(new MemoryCacheOptions()));
        //    var reportFormatter = new TableFormatter();
        //    var orderProcessor = new Processor(
        //        pricingEngine, customerCache, _parser, _lineSource, reportFormatter, _orderProcesserLogger);

        //    _parser.ParseLine(Arg.Any<string>()).Returns(new Order(
        //        id: 1,
        //        customer: new Customer("John Doe"),
        //        type: "Food",
        //        amount: 100.0,
        //        date: DateTime.Today,
        //        region: "US",
        //        state: "CA"));

        //    // Act
        //    var ordersReport = orderProcessor.ProcessOrders("test_orders.csv");

        //    // Assert
        //    Assert.IsNotNull(ordersReport);
        //    Assert.AreEqual(1, ordersReport.TotalOrders);
        //    Assert.AreEqual(100.0, ordersReport.TotalGross);
        //}

        //[TestMethod]
        //public void OrderProcessor_PrintOrdersReport_FormatsReportCorrectly()
        //{
        //    // Arrange
        //    var pricingEngine = new PricingEngine(_pricingEngineLogger);
        //    var customerCache = new InMemoryCustomerCache(new MemoryCache(new MemoryCacheOptions()));
        //    var reportFormatter = new TableFormatter();
        //    var orders = new List<Order>
        //    {
        //        new Order(1, new Customer("John Doe"), "Food", 100.0, _clock.Today(), "North", "CA")
        //    };
        //    var ordersReport = new OrdersReport(orders, pricingEngine);
        //    var orderProcessor = new Processor(
        //        pricingEngine, customerCache, _parser, new FileOrFallbackLineSource("test_orders.csv"), reportFormatter, _orderProcesserLogger);

        //    // Act
        //    var report = orderProcessor.PrintOrdersReport(ordersReport);

        //    // Assert
        //    Assert.IsFalse(string.IsNullOrWhiteSpace(report));
        //    Assert.Contains("John Doe", report);
        //}

        [TestMethod]
        public void NaiveCsvOrderParser_ParseLine_ParsesValidLineCorrectly()
        {
            // Arrange
            var parser = new NaiveCsvOrderParser(_csvOrderLogger, _clock);
            var line = "1,John Doe,Food,100.0,2025-11-10,North,CA";
            _clock.Today().Returns(new DateTime(2025, 11, 10));

            // Act
            var order = parser.ParseLine(line);

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual("John Doe", order.Customer.Name);
            Assert.AreEqual("Food", order.Type);
            Assert.AreEqual(100.0, order.Amount);
            Assert.AreEqual(new DateTime(2025, 11, 10), order.Date);
            Assert.AreEqual("North", order.Region);
            Assert.AreEqual("CA", order.State);
        }

        [TestMethod]
        public void NaiveCsvOrderParser_ParseLine_FailsOnInvalidLine()
        {
            // Arrange
            var parser = new NaiveCsvOrderParser(_csvOrderLogger, _clock);
            var line = "";
            _clock.Today().Returns(new DateTime(2025, 11, 10));

            // Act & Assert
            Assert.Throws<Exception>(() => parser.ParseLine(line));
        }
    }
}
