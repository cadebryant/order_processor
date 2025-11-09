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
            var app = BuildApplicationHost(path);

            var orderProcessor = app.Services.GetRequiredService<IOrderProcessor>();
            var orders = orderProcessor.ProcessOrders(path);
            var reportFormatter = app.Services.GetRequiredService<IReportFormatter>();
            var report = reportFormatter.FormatReport(orders);
            Console.WriteLine(report);
        }

        private static IHost BuildApplicationHost(string orderFilePath)
        {
            var applicationBuilder = Host.CreateApplicationBuilder();
            applicationBuilder.Services.AddLogging();
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
