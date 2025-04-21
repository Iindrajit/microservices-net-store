using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;
public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(); // Ensure a TraceId

        logger.LogInformation(
            @"[START] Handle request={Request} - Response={Response} - 
                RequestData={RequestData}. TraceId: {TraceId}",
            typeof(TRequest).Name, 
            typeof(TResponse).Name, 
            request, 
            traceId);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed.TotalSeconds;

        if (timeTaken > 3) // if the request is greater than 3 seconds, then log the warnings
            logger.LogWarning(
                "[PERFORMANCE] The request {Request} took {TimeTaken} seconds. TraceId: {TraceId}",
                typeof(TRequest).Name, 
                timeTaken,
                traceId);

        logger.LogInformation("[END] Handled {Request} with {Response}. TraceId: {TraceId}", 
            typeof(TRequest).Name, 
            typeof(TResponse).Name,
            traceId);

        return response;
    }
}