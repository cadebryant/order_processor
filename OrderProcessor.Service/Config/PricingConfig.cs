namespace OrderProcessor.Service.Config
{
    public class PricingConfig
    {
        public double FoodMultiplier { get; set; } = 0.9;
        public double ElectronicsMultiplier { get; set; } = 1.1;
        public double OtherMultiplier { get; set; } = 1.0;

        public double NyTax { get; set; } = 0.08875;
        public double CaTax { get; set; } = 0.0725;
        public double DefaultTax { get; set; } = 0.05;

        private Dictionary<string, double>? _typeMap;
        private Dictionary<string, double>? _stateTaxMap;

        public IReadOnlyDictionary<string, double> TypeMap =>
            _typeMap ??= new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["Food"] = FoodMultiplier,
                ["Electronics"] = ElectronicsMultiplier,
                ["Other"] = OtherMultiplier
            };

        public IReadOnlyDictionary<string, double> StateTaxMap =>
            _stateTaxMap ??= new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["NY"] = NyTax,
                ["CA"] = CaTax,
                ["Other"] = DefaultTax
            };

        public double GetPriceMultiplier(string itemType)
        {
            if (TypeMap.TryGetValue(itemType ?? string.Empty, out var multiplier))
            {
                return multiplier;
            }

            return OtherMultiplier;
        }

        public double GetStateTaxRate(string state)
        {
            if (StateTaxMap.TryGetValue(state ?? string.Empty, out var taxRate))
            {
                return taxRate;
            }

            return DefaultTax;
        }
    }
}
