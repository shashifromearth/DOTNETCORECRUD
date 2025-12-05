namespace InterviewAPI.Extensions;

/// <summary>
/// API Settings - demonstrates IOptions pattern for configuration
/// </summary>
public class ApiSettings
{
    public int MaxRequestsPerMinute { get; set; } = 100;
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}

