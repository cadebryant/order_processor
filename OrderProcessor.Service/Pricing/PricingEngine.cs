using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;


namespace OrderProcessor.Service.Pricing
{
    public sealed class PricingEngine(IOptions<PricingConfig> options) : IPricingEngine
    {
        private readonly PricingConfig _config = options.Value;

        private static readonly Dictionary<string, Func<PricingConfig, decimal>> TypeMultipliers
            = new(StringComparer.OrdinalIgnoreCase)
            {
                [OrderConstants.Types.Food] = c => c.FoodMultiplier,
                [OrderConstants.Types.Electronics] = c => c.ElectronicsMultiplier
            };

        private static readonly Dictionary<string, Func<PricingConfig, decimal>> StateTaxes
            = new(StringComparer.OrdinalIgnoreCase)
            {
                [OrderConstants.State.NY] = c => c.NyTax,
                [OrderConstants.State.CA] = c => c.CaTax
            };

        public PricingResult Price(Order order)
        {
            var multiplier = _config.GetPriceMultiplier(order.Type);
            var interim = order.Amount * (decimal)multiplier;

            var taxRate = _config.GetStateTaxRate(order.State);
            var net = interim + interim * (decimal)taxRate;
            return new PricingResult(decimal.Round(net, 2), decimal.Round(taxRate, 2));
        }
    }
}