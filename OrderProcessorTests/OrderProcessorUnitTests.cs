using LegacyOrderProcessor;
using OrderProcessor.Config;
using PricingConfig = OrderProcessor.Config.PricingConfig;
using Parser = OrderProcessor.Parsing.Parser;
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

        [TestMethod]
        public void Parser_ParseLine_ThrowsFormatException_ForInvalidInput()
        {
            // Arrange
            var invalidInput = "1,Ada Lovelace,Food,100.00,2024-07-01,US"; // Missing state part
            // Act & Assert
            Assert.ThrowsException<FormatException>(() => invalidInput.ParseLine());
        }

        [TestMethod]
        public void Parser_ParseLine_ThrowsFormatException_ForEmptyInput()
        {
            // Arrange
            var emptyInput = "";
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => emptyInput.ParseLine());
        }

        [TestMethod]
        public void Parser_ParseLine_ReturnsCorrectParts_ForValidInput()
        {
            // Arrange
            var validInput = "1,Ada Lovelace,Food,100.00,2024-07-01,US,NY";
            var expectedParts = ("1", "Ada Lovelace", "Food", "100.00", "2024-07-01", "US", "NY");
            // Act
            var actualParts = validInput.ParseLine();
            // Assert
            Assert.AreEqual(expectedParts, actualParts);
        }
    }
}
