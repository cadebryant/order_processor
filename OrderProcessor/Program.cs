using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessor.Service.Config;
using OrderProcessor.Service.Formatting;
using OrderProcessor.Service.IO;
using OrderProcessor.Service.Parsing;
using OrderProcessor.Service.Pricing;
using OrderProcessor.Service.Processing;
using OrdProcessor = OrderProcessor.Service.Processing.OrderProcessor;

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
