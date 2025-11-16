using OrderProcessor.Service.Config;
using OrderProcessor.Service.Parsing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Domain;
using Microsoft.Extensions.Caching.Memory;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.Pricing;
using Microsoft.Extensions.Options;

namespace OrderProcessorTests
{
    [TestClass]
    public sealed class OrderProcessorUnitTests
    {
        private readonly ILogger<PricingEngine> _pricingEngineLogger = NullLogger<PricingEngine>.Instance;
        private readonly ILogger<NaiveCsvOrderParser> _csvOrderLogger = NullLogger<NaiveCsvOrderParser>.Instance;
        private readonly IOrderParser _parser = Substitute.For<IOrderParser>();
        private readonly IClock _clock = Substitute.For<IClock>();
        private readonly ILineSource _lineSource = Substitute.For<ILineSource>();
        private readonly IOptions<PricingConfig> _pricingConfigOptions = Options.Create(new PricingConfig());
        private readonly PricingConfig _pricingConfig = new()
        {
            FoodMultiplier = 1.1m,
            ElectronicsMultiplier = 1.2m,
            OtherMultiplier = 1.3m,
            NyTax = 0.08m,
            CaTax = 0.075m,
            DefaultTax = 0.05m
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _clock.Today().Returns(DateTime.Today);
        }
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
                { OrderConstants.Types.Food, _pricingConfig.GetPriceMultiplier(OrderConstants.Types.Food) },
                { OrderConstants.Types.Electronics, _pricingConfig.GetPriceMultiplier(OrderConstants.Types.Electronics) },
                { OrderConstants.Types.Other, _pricingConfig.GetPriceMultiplier(OrderConstants.Types.Other) }
            };
            foreach (var testCase in testCases)
            {
                // Act
                var actualMultiplier = _pricingConfig.GetPriceMultiplier(testCase.Key);
                // Assert
                Assert.AreEqual(testCase.Value, actualMultiplier, $"Failed for item type: {testCase.Key}");
            }
        }

        [TestMethod]
        public void NaiveCsvOrderParser_ParseLine_ParsesValidLineCorrectly()
        {
            // Arrange
            var parser = new NaiveCsvOrderParser(_clock);
            var line = "1,John Doe,Food,100.0,2025-11-10,North,CA";
            _clock.Today().Returns(new DateTime(2025, 11, 10));

            // Act
            //var order = parser.ParseLine(line);
            parser.TryParse(line, out var order);

            // Assert
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
            var parser = new NaiveCsvOrderParser(_clock);
            var line = "";
            _clock.Today().Returns(new DateTime(2025, 11, 10));

            // Act & Assert
            Assert.IsFalse(parser.TryParse(line, out var _));
        }
    }
}
