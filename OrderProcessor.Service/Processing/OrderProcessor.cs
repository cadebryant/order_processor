using Microsoft.Extensions.Logging;
using OrderProcessor.Service.Domain;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Pricing;

namespace OrderProcessor.Service.Processing
{
    public class OrderProcessor(
        IPricingEngine pricingEngine,
        ICustomerCache customerCache,
        IOrderParser orderParser,
        ILineSource lineSource,
        IReportFormatter reportFormatter,
        ILogger<OrderProcessor> logger) : IOrderProcessor
    {
        private readonly IPricingEngine _pricingEngine = pricingEngine;
        private readonly ICustomerCache _customerCache = customerCache;
        private readonly IOrderParser _orderParser = orderParser;
        private readonly ILineSource _lineSource = lineSource;
        private readonly IReportFormatter _reportFormatter = reportFormatter;
        private readonly ILogger _logger = logger;

        public OrdersReport ProcessOrders(string orderFilePath)
        {
            try
            {
                var orderLines = _lineSource.GetLines();
                var parsedOrders = orderLines
                    .Select(_orderParser.ParseLine)
                    .ToList();

                foreach (var order in parsedOrders)
                {
                    var customer = _customerCache.GetCustomer(order.Customer.Name) ?? order.Customer;
                    order.Net = _pricingEngine.CalculateNetPrice(order);
                }
                return new OrdersReport(parsedOrders, _pricingEngine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order file {OrderFilePath}", orderFilePath);
                throw;
            }
        }

        public string PrintOrdersReport(OrdersReport report)
        {
            return _reportFormatter.FormatReport(report);
        }
    }
}
