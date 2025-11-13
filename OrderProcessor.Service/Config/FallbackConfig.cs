using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Config
{
    public class FallbackConfig
    {
        public IEnumerable<string> FallbackData { get; set; } = [];
    }
}
