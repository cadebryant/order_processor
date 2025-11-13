using Microsoft.Extensions.Logging;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Pricing
{
    public class PricingEngine(ILogger<PricingEngine> logger) : IPricingEngine
    {
        private readonly ILogger<PricingEngine> _logger = logger;
        public double CalculateDiscountOrSurcharge(Order order)
        {
            try
            {
                return order == null 
                    ? throw new ArgumentNullException(nameof(order)) 
                    : PricingConfig.GetPriceMultiplier(order.Type);
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
                return amount * PricingConfig.GetStateTaxRate(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating tax amount for order {id}", id);
                throw;
            }
        }
    }
}
