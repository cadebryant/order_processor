using OrderProcessor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.App
{
    public interface IOrderProcessor
    {
        OrdersReport ProcessOrders(string orderFilePath);
        string PrintOrdersReport(OrdersReport report);
    }
}
