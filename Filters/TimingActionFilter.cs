using Microsoft.AspNetCore.Mvc.Filters;

namespace InterviewAPI.Filters;

/// <summary>
/// Action Filter for request timing - demonstrates ActionFilterAttribute
/// Logs the duration of each request for performance monitoring
/// </summary>
public class TimingActionFilter : IActionFilter
{
    private readonly ILogger<TimingActionFilter> _logger;
    private const string StartTimeKey = "StartTime";

    public TimingActionFilter(ILogger<TimingActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Store start time before action executes
        context.HttpContext.Items[StartTimeKey] = DateTime.UtcNow;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Calculate duration after action executes
        if (context.HttpContext.Items.TryGetValue(StartTimeKey, out var startTimeObj) && startTimeObj is DateTime startTime)
        {
            var duration = DateTime.UtcNow - startTime;
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            
            _logger.LogInformation(
                "Request to {Controller}.{Action} took {Duration}ms",
                controller, action, duration.TotalMilliseconds);
        }
    }
}

