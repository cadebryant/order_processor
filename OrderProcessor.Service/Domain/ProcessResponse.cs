using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Domain
{
    public sealed record ProcessResponse(
        string ReportId,
        string ReportText)
    {
        public int Orders { get; init; }
        public decimal Gross { get; init; }
        public decimal Revenue { get; init; }
    }
}
