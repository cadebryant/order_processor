using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Parsing
{
    internal interface IOrderParser
    {
        (string id, string customer, string type, string amount, string date, string region, string state) ParseLine(string l);
    }
}
