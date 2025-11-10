using OrderProcessor.Domain;
using System.Globalization;
using System.Text;

namespace OrderProcessor.Formatting
{
    internal class TableFormatter : IReportFormatter
    {
        public string FormatReport(OrdersReport reportData)
        {
            var sb = new StringBuilder();
            sb.AppendLine(PrintHeader());
            foreach (var order in reportData.Orders)
            {
                sb.AppendLine($"{order.Id} | {order.Customer.Name} | {order.Type} | {order.Amount.ToString("0.00", CultureInfo.InvariantCulture)} | {order.Date:yyyy-MM-dd} | {order.Region} | {order.State} | {order.Net.ToString("0.00", CultureInfo.InvariantCulture)} | {order.Note}");
            }
            sb.AppendLine($"Total Orders: {reportData.TotalOrders}");
            sb.AppendLine($"Gross: {reportData.TotalGross.ToString("0.00", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Revenue: {reportData.Revenue.ToString("0.00", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Avg Net/Order: {reportData.AverageNet.ToString("0.00", CultureInfo.InvariantCulture)}");

            return sb.ToString();
        }

        private static string PrintHeader()
        {
            return "ID | Customer | Type | Amount | Date | Region | State | Net | Note";
        }
    }
}
