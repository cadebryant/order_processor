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
using Microsoft.Extensions.Caching.Memory;

namespace OrderProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var orderFilePath = args.Length > 0 ? args[0] : "orders.csv";
                var app = BuildApplicationHost(orderFilePath);

                var orderProcessor = app.Services.GetRequiredService<IOrderProcessor>();
                var orders = orderProcessor.ProcessOrders(orderFilePath);
                var report = orderProcessor.PrintOrdersReport(orders);
                Console.WriteLine(report);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static IHost BuildApplicationHost(string orderFilePath)
        {
            var applicationBuilder = Host.CreateApplicationBuilder();

            // Add logging
            applicationBuilder.Services.AddLogging();

            // Register IMemoryCache
            applicationBuilder.Services.AddMemoryCache();

            // Register other services
            applicationBuilder.Services.AddScoped<ILineSource>(sp => new FileOrFallbackLineSource(orderFilePath));
            applicationBuilder.Services.AddScoped<IOrderParser, NaiveCsvOrderParser>();
            applicationBuilder.Services.AddScoped<IClock, SystemClock>();
            applicationBuilder.Services.AddScoped<IReportFormatter, TableFormatter>();
            applicationBuilder.Services.AddScoped<IPricingEngine, PricingEngine>();
            applicationBuilder.Services.AddScoped<ICustomerCache, InMemoryCustomerCache>();
            applicationBuilder.Services.AddScoped<IOrderProcessor, OrdProcessor>();

            return applicationBuilder.Build();
        }
    }
}
