using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public interface IPricingEngine
    {
        double CalculateNetPrice(Order order);
        double CalculateGrossPrice(Order order);
        double CalculateTotalPrice(Order order);
        double CalculateDiscountOrSurcharge(Order order);
        double CalculateTaxAmount(Order order);
    }
}
