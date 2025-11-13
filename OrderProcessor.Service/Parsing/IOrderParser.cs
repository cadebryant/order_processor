using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Parsing
{
    public interface IOrderParser
    {
        Order ParseLine(string l);
    }
}
