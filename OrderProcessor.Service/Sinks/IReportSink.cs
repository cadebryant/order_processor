using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Sinks
{
    public interface IReportSink
    {
        Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken ct);
        Task<bool> ExistsAsync(string path, CancellationToken ct);
        Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct) ;
    }
}
