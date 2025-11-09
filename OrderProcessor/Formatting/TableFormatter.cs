using OrderProcessor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                sb.AppendLine($"{order.Id} | {order.Customer.FirstName} {order.Customer.LastName} | {order.Type} | {order.Amount:C} | {order.Date:yyyy-MM-dd} | {order.Region} | {order.State} | {order.Net} | {order.Note}");
            }
            sb.AppendLine($"Total Orders: {reportData.TotalOrders}");
            sb.AppendLine($"Gross: {reportData.TotalGross}");
            sb.AppendLine($"Revenue: {reportData.Revenue}");
            sb.AppendLine($"Avg Net/Order: {reportData.AverageNet}");

            return sb.ToString();
        }

        private string PrintHeader()
        {
            return "ID | Customer | Type | Amount | Date | Region | State | Net | Note";
        }
    }
}
