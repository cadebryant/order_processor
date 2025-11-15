namespace OrderProcessor.Service.Config
{
    public class PricingConfig
    {
        public const string SectionName = "Pricing";

        public decimal FoodMultiplier { get; set; }
        public decimal ElectronicsMultiplier { get; set; }
        public decimal OtherMultiplier { get; set; }
        public decimal NyTax { get; set; }
        public decimal CaTax { get; set; }
        public decimal DefaultTax { get; set; }
        private Dictionary<string, decimal>? _typeMap;
        private Dictionary<string, decimal>? _stateTaxMap;

        public IReadOnlyDictionary<string, decimal> TypeMap =>
            _typeMap ??= new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                [OrderConstants.Types.Food] = FoodMultiplier,
                [OrderConstants.Types.Electronics] = ElectronicsMultiplier,
                [OrderConstants.Types.Other] = OtherMultiplier
            };

        public IReadOnlyDictionary<string, decimal> StateTaxMap =>
            _stateTaxMap ??= new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                [OrderConstants.State.NY] = NyTax,
                [OrderConstants.State.CA] = CaTax,
                [OrderConstants.State.Other] = DefaultTax
            };

        public decimal GetPriceMultiplier(string itemType)
        {
            if (TypeMap.TryGetValue(itemType ?? string.Empty, out var multiplier))
            {
                return multiplier;
            }

            return OtherMultiplier;
        }

        public decimal GetStateTaxRate(string state)
        {
            if (StateTaxMap.TryGetValue(state ?? string.Empty, out var taxRate))
            {
                return taxRate;
            }

            return DefaultTax;
        }
    }
}
