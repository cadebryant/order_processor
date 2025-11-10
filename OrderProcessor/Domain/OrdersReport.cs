using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public class OrdersReport(IEnumerable<Order> orders, IPricingEngine pricingEngine)
    {
        public IEnumerable<Order> Orders { get; init; } = orders;
        public int TotalOrders => Orders.Count();
        public double TotalGross => Orders.Sum(o => pricingEngine.CalculateGrossPrice(o));
        public double Revenue => Orders.Sum(o => o.Net);
        public double AverageNet => TotalOrders == 0 ? 0.0 : Revenue / TotalOrders;
    }
}
