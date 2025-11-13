using OrderProcessor.Service.Domain;

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
