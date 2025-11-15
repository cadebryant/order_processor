using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Domain
{
    public sealed record PricingResult(decimal Net, decimal TaxRate);
}
