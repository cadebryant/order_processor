using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Domain
{
    public sealed class ProcessRequest
    {
        public string? Csv { get; set; }
    }
}
