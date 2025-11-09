using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public class PricingResult(double net, double taxRate, string? note)
    {
        public double Net { get; set; } = net;
        public double TaxRate { get; set; } = taxRate;
        public string? Note { get; set; } = note;
    }
}
