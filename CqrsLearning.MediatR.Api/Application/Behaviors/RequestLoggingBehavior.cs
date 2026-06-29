using System.Diagnostics;
using MediatR;

namespace CqrsLearning.MediatR.Api.Application.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling MediatR request {RequestName}", requestName);

        var response = await next();

        stopwatch.Stop();

        _logger.LogInformation(
            "Handled MediatR request {RequestName} in {ElapsedMilliseconds} ms",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
