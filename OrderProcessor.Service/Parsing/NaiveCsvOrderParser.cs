using Microsoft.Extensions.Logging;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.IO;

namespace OrderProcessor.Service.Parsing
{
    public class NaiveCsvOrderParser(
        ILogger<NaiveCsvOrderParser> logger,
        IClock clock) : IOrderParser
    {
        private readonly ILogger<NaiveCsvOrderParser> _logger = logger;
        private readonly IClock _clock = clock;
        public Order ParseLine(string l)
        {
            try
            {
                if (!IsValidCsvLine(l))
                    throw new ArgumentException("Input line cannot be null or empty.", nameof(l));

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
                _logger.LogError(ex, "Error parsing line: {Line}", l);
                throw;
            }
        }

        private DateTime ParseDate(string dateString)
        {
            try
            {
                if (long.TryParse(dateString, out var ticks))
                {
                    return new DateTime(ticks);
                }
                if (DateTime.TryParse(dateString, out var parsedDate))
                {
                    return parsedDate;
                }
                return _clock.Today();
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse date: {dateString}", ex);
                throw;
            }
        }

        private static string GetItemOrEmpty(string[] parts, int index)
        {
            return (parts != null && parts.Length > index) ? parts[index] : string.Empty;
        }

        private static bool IsValidCsvLine(string line)
        {
            return !string.IsNullOrWhiteSpace(line) &&
                   (line.Contains(',') || line.Split(',').Length == 1);
        }
    }
}
