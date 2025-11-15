using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Domain
{
    public sealed class ProcessResponse
    {
        public int TotalOrders { get; set; }
        public decimal Gross { get; set; }
        public decimal Revenue { get; set; }
        public IEnumerable<string>? Lines { get; set; }
        public string? Message { get; set; } = null;
    }
}
