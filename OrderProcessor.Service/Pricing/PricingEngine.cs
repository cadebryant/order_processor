using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Pricing
{
    public class PricingEngine(ILogger<PricingEngine> logger, IOptions<PricingConfig> pricingConfigOptions) : IPricingEngine
    {
        private readonly ILogger<PricingEngine> _logger = logger;
        private readonly PricingConfig _pricingConfig = pricingConfigOptions.Value;
        public double CalculateDiscountOrSurcharge(Order order)
        {
            try
            {
                return order == null 
                    ? throw new ArgumentNullException(nameof(order)) 
                    : _pricingConfig.GetPriceMultiplier(order.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating discount or surcharge for order {OrderId}", order?.Id);
                throw;
            }
        }

        public double CalculateGrossPrice(Order order)
        {
            try
            {
                return order == null
                    ? throw new ArgumentNullException(nameof(order))
                    : order.Amount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating gross price for order {OrderId}", order?.Id);
                throw;
            }
        }

        public double CalculateNetPrice(Order order)
        {
            try
            {
                return order == null
                    ? throw new ArgumentNullException(nameof(order))
                    : (order.Amount * CalculateDiscountOrSurcharge(order)) 
                        + CalculateTaxAmount(order.Id, order.State, order.Amount * CalculateDiscountOrSurcharge(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating net price for order {OrderId}", order?.Id);
                throw;
            }
        }

        public double CalculateTaxAmount(int id, string state, double amount)
        {
            try
            {
                return amount * _pricingConfig.GetStateTaxRate(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating tax amount for order {id}", id);
                throw;
            }
        }
    }
}
