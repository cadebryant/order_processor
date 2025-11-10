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
                if (parts == null || parts.Length != 7)
                {
                    throw new FormatException("Input line is not in the expected CSV format.");
                }

                var customer = new Customer(parts[1]);
                return new Order(
                    id: int.TryParse(parts[0], out var id) ? id : -1,
                    customer: customer,
                    type: parts[2],
                    amount: double.Parse(parts[3]),
                    date: ParseDate(parts[4]),
                    region: parts[5],
                    state: parts[6]
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
    }
}
