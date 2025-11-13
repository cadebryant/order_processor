using OrderProcessor.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Processing
{
    public interface IOrderProcessor
    {
        OrdersReport ProcessOrders(string orderFilePath);
        string PrintOrdersReport(OrdersReport report);
    }
}
