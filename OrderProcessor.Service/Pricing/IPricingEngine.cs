using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Pricing
{
    public interface IPricingEngine
    {
        PricingResult Price(Order order);
    }
}
