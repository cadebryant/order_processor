using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessor.App;
using OrderProcessor.Domain;
using OrderProcessor.Formatting;
using OrderProcessor.IO;
using OrderProcessor.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrdProcessor = OrderProcessor.App.OrderProcessor;

namespace OrderProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var path = args.Length > 0 ? args[0] : "orders.csv";

            var applicationBuilder = Host.CreateApplicationBuilder(args);
            applicationBuilder.Services.AddLogging();
            applicationBuilder.Services.AddScoped<ILineSource>(sp => new FileOrFallbackLineSource(path));
            applicationBuilder.Services.AddScoped<IOrderParser, NaiveCsvOrderParser>();
            applicationBuilder.Services.AddScoped<IClock, SystemClock>();
            applicationBuilder.Services.AddScoped<IReportFormatter, TableFormatter>();
            applicationBuilder.Services.AddScoped<IPricingEngine, PricingEngine>();
            applicationBuilder.Services.AddScoped<ICustomerCache, InMemoryCustomerCache>();
            applicationBuilder.Services.AddScoped<IOrderProcessor, OrdProcessor>();
            var app = applicationBuilder.Build();

            var orderProcessor = app.Services.GetRequiredService<IOrderProcessor>();
            var result = orderProcessor.ProcessOrder(path);
        }
    }
}
