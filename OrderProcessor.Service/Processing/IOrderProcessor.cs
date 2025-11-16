using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Processing
{
    public interface IOrderProcessor
    {
        Task<Results<Ok<ProcessResponse>, ValidationProblem>> ProcessOrdersAsync(HttpRequest processOrdersRequest, CancellationToken ct);
        Task<IResult> GetOrderReportAsync(string id, CancellationToken ct);
    }
}
