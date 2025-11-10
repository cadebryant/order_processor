using Microsoft.Extensions.Logging;
using OrderProcessor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingConfig = OrderProcessor.Config.PricingConfig;

namespace OrderProcessor.Domain
{
    public class PricingEngine(ILogger<PricingEngine> logger) : IPricingEngine
    {
        private ILogger<PricingEngine> _logger = logger;
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
