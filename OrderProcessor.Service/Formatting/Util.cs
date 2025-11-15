using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Formatting
{
    public static class Util
    {
        public static string[] SplitLines(this string raw) =>
            raw.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);

        public static bool LooksLikeHeader(this string? first)
        {
            if (string.IsNullOrWhiteSpace(first)) return false;
            var f = first.Trim().ToLowerInvariant();
            return f.Contains("id") || f.Contains("customer");
        }
    }
}
