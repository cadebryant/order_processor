using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Parsing
{
    public class NaiveCsvOrderParser : IOrderParser
    {
        public (string id, string customer, string type, string amount, string date, string region, string state) ParseLine(string l)
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

            return (parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6]);
        }
    }
}
