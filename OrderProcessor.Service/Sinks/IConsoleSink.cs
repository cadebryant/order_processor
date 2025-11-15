using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Sinks
{
    public interface IConsoleSink
    {
        void Write(string s);
    }
}
