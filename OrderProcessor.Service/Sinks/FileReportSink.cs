using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Sinks
{
    public sealed class FileReportSink : IReportSink
    {
        public async Task<bool> ExistsAsync(string path, CancellationToken ct) 
            => await Task.FromResult(File.Exists(path));

        public async Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct) 
            => await File.ReadAllLinesAsync(path, ct);

        public async Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken ct)
        {
            var fullPath = Path.GetFullPath(path);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllLinesAsync(fullPath, lines, ct);
        }
    }
}
