namespace OrderProcessor.Service.Domain
{
    public sealed record Order(
        string Id, 
        string Customer, 
        string Type, 
        decimal Amount, 
        DateTime Date, 
        string Region, 
        string State);
}
