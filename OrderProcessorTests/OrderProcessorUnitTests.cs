using OrderProcessor;
using OrderProcessor.Config;
using PricingConfig = OrderProcessor.Config.PricingConfig;
using OrderProcessor.Parsing;

namespace OrderProcessorTests
{
    [TestClass]
    public sealed class OrderProcessorUnitTests
    {
        [TestMethod]
        public void PricingConfig_TypeMap_ReturnsOther_IfItemTypeNotFound()
        {
            // Arrange
            var unknownItemType = "Toys";
            var expectedMultiplier = PricingConfig.GetPriceMultiplier(PricingConfig.Types.Other);
            // Act
            var actualMultiplier = PricingConfig.GetPriceMultiplier(unknownItemType);
            // Assert
            Assert.AreEqual(expectedMultiplier, actualMultiplier);
        }

        [TestMethod]
        public void PricingConfig_TypeMap_ReturnsCorrectMultiplier_ForKnownItemTypes()
        {
            // Arrange
            var testCases = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { PricingConfig.Types.Food, PricingConfig.Types.FoodMultiplier },
                { PricingConfig.Types.Electronics, PricingConfig.Types.ElectronicsMultiplier },
                { PricingConfig.Types.Other, PricingConfig.Types.OtherMultiplier }
            };
            foreach (var testCase in testCases)
            {
                // Act
                var actualMultiplier = PricingConfig.GetPriceMultiplier(testCase.Key);
                // Assert
                Assert.AreEqual(testCase.Value, actualMultiplier, $"Failed for item type: {testCase.Key}");
            }
        }
    }
}
