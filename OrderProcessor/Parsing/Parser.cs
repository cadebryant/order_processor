using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Parsing
{
    public static class Parser
    {
        public static (string id, string customer, string type, string amount, string date, string region, string state) ParseLine(
            this string input,
            char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input line cannot be null or whitespace.", nameof(input));
            }

            var parts = input.Split(separator);
            if (parts.Length != 7)
            {
                throw new FormatException("Input line is not in the expected format.");
            }

            return (parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6]);
        }
    }
}
