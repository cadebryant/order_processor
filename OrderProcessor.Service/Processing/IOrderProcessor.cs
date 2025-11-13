using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Processing
{
    public interface IOrderProcessor
    {
        OrdersReport ProcessOrders(string orderFilePath);
        string PrintOrdersReport(OrdersReport report);
    }
}
