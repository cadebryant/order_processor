using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Parsing
{
    public interface IOrderParser
    {
        bool TryParse(string line, out Order order);
    }
}
