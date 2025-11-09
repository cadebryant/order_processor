using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Config
{
    public static class PricingConfig
    {
        public static class Types
        {
            public const string Food = nameof(Food);
            public const string Electronics = nameof(Electronics);
            public const string Other = nameof(Other);
            public const double FoodMultiplier = 0.9;
            public const double ElectronicsMultiplier = 1.1;
            public const double OtherMultiplier = 1.0;
        }

        public static class DiscountOrSurcharge
        {
            public const double FoodTaxRate = 0.05;
            public const double ElectronicsTaxRate = 0.15;
            public const double OtherTaxRate = 0.1;
        }

        public static class State
        {
            public const string NY = nameof(NY);
            public const string CA = nameof(CA);
            public const string Other = nameof(Other);
        }

        public static class Region
        {
            public const string US = nameof(US);
            public const string EU = nameof(EU);
        }

        public static class TaxRate
        {
            public const double NYTaxRate = 0.0875;
            public const double CATaxRate = 0.0725;
            public const double OtherTaxRate = 0.05;
        }

        public static readonly List<string> DefaultOrderFallbackLines =
        [
            "1,Ada Lovelace,Food,100.00,2024-07-01,US,NY",
            "2,Grace Hopper,Electronics,250.49,7/4/2024,US,CA",
            "3,Alan Turing,Other,-42,16908480000000000,EU,",
            "4,Katherine Johnson,Food,0.00,2024-10-15,US,TX",
            "5,Grace Hopper,Other,10.25,2024-12-31,US,WA"
        ];

        public static readonly Dictionary<string, double> TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {Types.Food, Types.FoodMultiplier},
            {Types.Electronics, Types.ElectronicsMultiplier},
            {Types.Other, Types.OtherMultiplier}
        };

        public static readonly Dictionary<string, double> StateTaxMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {State.NY, TaxRate.NYTaxRate},
            {State.CA, TaxRate.CATaxRate},
            {State.Other, TaxRate.OtherTaxRate},
        };

        public static double GetPriceMultiplier(string itemType)
        {
            if (TypeMap.TryGetValue(itemType, out var multiplier))
            {
                return multiplier;
            }
            return TypeMap[Types.Other];
        }

        public static double GetStateTaxRate(string state)
        {
            if (StateTaxMap.TryGetValue(state, out var taxRate))
            {
                return taxRate;
            }
            return TaxRate.OtherTaxRate;
        }
    }
}
