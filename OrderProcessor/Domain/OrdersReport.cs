using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public class OrdersReport
    {
        public OrdersReport(IEnumerable<Order> orders)
        {
            Orders = orders;
        }
        public IEnumerable<Order> Orders { get; set; }
        public int TotalOrders => Orders.Count();
        public double TotalGross => Orders.Sum(o => o.Amount);
        public double Revenue => Orders.Sum(o => o.Amount * o.DiscountOrSurcharge * o.Tax);
        public double AverageNet => TotalOrders == 0 ? Revenue / TotalOrders : 0.0;
    }
}
