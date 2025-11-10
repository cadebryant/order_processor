using OrderProcessor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Parsing
{
    public class NaiveCsvOrderParser : IOrderParser
    {
        public Order ParseLine(string l)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(l))
                {
                    throw new ArgumentException("Input line cannot be null or empty.", nameof(l));
                }

                var parts = l.Split(',');
                var customer = new Customer(GetItemOrEmpty(parts, 1));

                return new Order(
                    id: int.TryParse(GetItemOrEmpty(parts, 0), out var id) ? id : -1,
                    customer: customer,
                    type: GetItemOrEmpty(parts, 2),
                    amount: double.TryParse(GetItemOrEmpty(parts, 3), out var amt) ? amt : 0.0,
                    date: ParseDate(GetItemOrEmpty(parts, 4)),
                    region: GetItemOrEmpty(parts, 5),
                    state: GetItemOrEmpty(parts, 6)
                );
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse line: {l}", ex);
                throw;
            }
        }

        private static DateTime ParseDate(string dateString)
        {
            try
            {
                if (long.TryParse(dateString, out var ticks))
                {
                    return new DateTime(ticks);
                }
                return DateTime.Parse(dateString);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse date: {dateString}", ex);
                throw;
            }
        }

        private string GetItemOrEmpty(string[] parts, int index)
        {
            return (parts != null && parts.Length > index) ? parts[index] : string.Empty;
        }
    }
}
