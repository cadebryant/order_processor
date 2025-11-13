using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Formatting
{
    public interface IReportFormatter
    {
        string FormatReport(OrdersReport reportData);
    }
}
