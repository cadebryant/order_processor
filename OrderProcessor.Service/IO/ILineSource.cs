using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.IO
{
    public interface ILineSource
    {
        IEnumerable<string> GetLines();
    }
}
