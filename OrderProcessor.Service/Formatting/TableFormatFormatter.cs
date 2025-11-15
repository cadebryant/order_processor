using OrderProcessor.Service.Domain;
using System.Globalization;
using System.Text;

namespace OrderProcessor.Service.Formatting
{
    public class TableFormatFormatter : IReportFormatter
    {
        public string Header => throw new NotImplementedException();

        public string FormatRow(Order order, PricingResult pricingResult)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FormatSummary(int count, double gross, double revenue)
        {
            throw new NotImplementedException();
        }
    }
}
