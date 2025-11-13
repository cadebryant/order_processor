using OrderProcessor.Service.Config;

namespace OrderProcessor.Service.IO
{
    public class FileOrFallbackLineSource(string filePath) : ILineSource
    {
        private readonly string _filePath = filePath;

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
