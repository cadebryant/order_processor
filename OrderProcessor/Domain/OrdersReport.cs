namespace OrderProcessor.Domain
{
    public class OrdersReport(IEnumerable<Order> orders, IPricingEngine pricingEngine)
    {
        public IEnumerable<Order> Orders { get; init; } = orders;
        public int TotalOrders => Orders.Count();
        public double TotalGross => Orders.Sum(pricingEngine.CalculateGrossPrice);
        public double Revenue => Orders.Sum(o => o.Net);
        public double AverageNet => 
            TotalOrders == 0 
                ? 0.0 
                : Revenue / TotalOrders;
    }
}
