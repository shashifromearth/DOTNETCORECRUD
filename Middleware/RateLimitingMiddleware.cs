using System.Collections.Concurrent;

namespace InterviewAPI.Middleware;

/// <summary>
/// Rate Limiting Middleware - demonstrates custom middleware implementation
/// Limits requests per IP address using in-memory storage
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestHistory = new();
    private const int MaxRequestsPerMinute = 100;
    private const int TimeWindowMinutes = 1;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Clean old entries
        CleanOldEntries(clientIp);

        // Check rate limit
        if (_requestHistory.TryGetValue(clientIp, out var requests))
        {
            var recentRequests = requests.Count(r => r > DateTime.UtcNow.AddMinutes(-TimeWindowMinutes));
            
            if (recentRequests >= MaxRequestsPerMinute)
            {
                _logger.LogWarning("Rate limit exceeded for IP: {Ip}", clientIp);
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 429,
                    Message = "Rate limit exceeded. Please try again later.",
                    RetryAfter = TimeWindowMinutes * 60 // seconds
                });
                return;
            }
        }

        // Record request
        _requestHistory.AddOrUpdate(
            clientIp,
            new List<DateTime> { DateTime.UtcNow },
            (key, existing) =>
            {
                existing.Add(DateTime.UtcNow);
                return existing;
            });

        await _next(context);
    }

    private void CleanOldEntries(string clientIp)
    {
        if (_requestHistory.TryGetValue(clientIp, out var requests))
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TimeWindowMinutes);
            requests.RemoveAll(r => r < cutoff);
            
            if (requests.Count == 0)
            {
                _requestHistory.TryRemove(clientIp, out _);
            }
        }
    }
}

