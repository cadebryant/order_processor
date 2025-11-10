using OrderProcessor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Parsing
{
    public interface IOrderParser
    {
        Order ParseLine(string l);
    }
}
