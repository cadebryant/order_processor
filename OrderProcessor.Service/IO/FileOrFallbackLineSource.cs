using Microsoft.Extensions.Options;
using OrderProcessor.Service.Config;

namespace OrderProcessor.Service.IO
{
    public class FileOrFallbackLineSource(string filePath, IOptions<FallbackConfig> fallbackConfigOptions) : ILineSource
    {
        private readonly string _filePath = filePath;
        private readonly FallbackConfig _fallbackConfig = fallbackConfigOptions.Value;

        public IEnumerable<string> GetLines()
        {
            try
            {
                return File.ReadLines(_filePath);
            }
            catch (FileNotFoundException)
            {
                return _fallbackConfig.FallbackData;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving lines.", ex);
            }
        }
    }
}
