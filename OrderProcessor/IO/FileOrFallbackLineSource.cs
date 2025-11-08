using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OrderProcessor.Config;
using PricingConfig = OrderProcessor.Config.PricingConfig;

namespace OrderProcessor.IO
{
    public class FileOrFallbackLineSource : ILineSource
    {
        private readonly string _filePath;

        public FileOrFallbackLineSource(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<string> GetLines()
        {
            try
            {
                return File.ReadLines(_filePath);
            }
            catch (FileNotFoundException)
            {
                return PricingConfig.DefaultOrderFallbackLines;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving lines.", ex);
            }
        }
    }
}
