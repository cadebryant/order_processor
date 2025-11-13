using OrderProcessor.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Parsing
{
    public interface IOrderParser
    {
        Order ParseLine(string l);
    }
}
