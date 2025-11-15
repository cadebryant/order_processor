using OrderProcessor.Service.Domain;
using System.Globalization;
using System.Text;

namespace OrderProcessor.Service.Formatting
{
    public sealed class TableReportFormatter : IReportFormatter
    {
        public string Header => "ID | Customer | Type | Amount | Date | Region | State | Net | Note";
        public string FormatRow(Order o, PricingResult r)
            => $"{o.Id} | {o.Customer} | {o.Type} | {o.Amount.ToString("0.00", CultureInfo.InvariantCulture)} | {o.Date:yyyy-MM-dd} | {o.Region} | {o.State} | {r.Net.ToString("0.00", CultureInfo.InvariantCulture)} |";
        public IEnumerable<string> FormatSummary(int count, double gross, double revenue)
        {
            yield return new string('-', 95);
            yield return $"Total Orders: {count}";
            yield return $"Gross: {gross:0.00}";
            yield return $"Revenue: {revenue:0.00}";
            yield return $"Avg Net/Order: {(count == 0 ? 0 : revenue / count):0.00}";
        }
    }
}
