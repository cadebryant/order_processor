using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.IO
{
    public interface IOrderFileReader
    {
        IEnumerable<string> ReadLines(string filePath);
    }
}
