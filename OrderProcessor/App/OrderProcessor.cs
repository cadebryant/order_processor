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

        public PricingResult ProcessOrder(string orderFilePath)
        {
            try
            {
                return new PricingResult(0, 0, "Not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order file {OrderFilePath}", orderFilePath);
                throw;
            }
        }
    }
}
