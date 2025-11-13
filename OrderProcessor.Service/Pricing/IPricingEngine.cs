using OrderProcessor.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Pricing
{
    public interface IPricingEngine
    {
        double CalculateNetPrice(Order order);
        double CalculateGrossPrice(Order order);
        double CalculateDiscountOrSurcharge(Order order);
        double CalculateTaxAmount(int id, string state, double amount);
    }
}
