using Microsoft.Extensions.Logging;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.IO;
using System.Globalization;

namespace OrderProcessor.Service.Parsing
{
    public sealed class NaiveCsvOrderParser(
        IClock clock) : IOrderParser
    {
        private readonly IClock _clock = clock;

        public bool TryParse(string line, out Order order)
        {
            var parts = line.Split(',');

            while (parts.Length < 7)
                parts = [.. parts, .. new[] { "" }];

            var (id, customer, type, amount, date, region, state)
                = (parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6]);
            if (!decimal.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var amt))
                amt = 0m;
            if (!TryParseDate(date, out var dt))
                dt = _clock.Today();
            order = new Order(id, customer, type, amt, dt, region, state);
            return true;
        }

        private static bool TryParseDate(string s, out DateTime dt)
        {
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;
            if (DateTime.TryParse(s, out dt))
                return true;
            if (long.TryParse(s, out var ticks))
            {
                try { dt = new DateTime(ticks); return true; } catch { /* fallthrough */ }
            }
            return false;
        }
    }
}
