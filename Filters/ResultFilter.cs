using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InterviewAPI.Filters;

/// <summary>
/// Result Filter - demonstrates IResultFilter for response formatting
/// Can be used to modify responses before they are sent to the client
/// </summary>
public class ResultFilter : IResultFilter
{
    private readonly ILogger<ResultFilter> _logger;

    public ResultFilter(ILogger<ResultFilter> logger)
    {
        _logger = logger;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Can modify response before it's executed
        if (context.Result is ObjectResult objectResult)
        {
            _logger.LogDebug("Result type: {Type}, StatusCode: {StatusCode}",
                objectResult.Value?.GetType().Name, objectResult.StatusCode);
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // Can perform actions after result is executed
        if (context.Result is ObjectResult objectResult)
        {
            _logger.LogDebug("Result executed successfully");
        }
    }
}

