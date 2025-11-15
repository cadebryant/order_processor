using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Formatting
{
    public interface IReportFormatter
    {
        string Header { get; }
        string FormatRow(Order order, PricingResult pricingResult);
        IEnumerable<string> FormatSummary(int count, double gross, double revenue);
    }
}
