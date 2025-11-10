using Microsoft.Extensions.Logging;
using OrderProcessor.Domain;
using OrderProcessor.Formatting;
using OrderProcessor.IO;
using OrderProcessor.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingConfig = OrderProcessor.Config.PricingConfig;

namespace OrderProcessor.App
{
    public class OrderProcessor(
        IClock clock,
        IPricingEngine pricingEngine,
        ICustomerCache customerCache,
        IOrderParser orderParser,
        ILineSource lineSource,
        IReportFormatter reportFormatter,
        ILogger<OrderProcessor> logger) : IOrderProcessor
    {
        private readonly IClock _clock = clock;
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
                    if (order.Date == null)
                        order.Date = _clock.Today();
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
