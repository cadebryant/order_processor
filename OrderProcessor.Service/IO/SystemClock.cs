using OrderProcessor.Service.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.IO
{
    public class SystemClock : IClock
    {
        public DateTime Today() => DateTime.Today;
    }
}
