namespace InterviewAPI.DTOs;

/// <summary>
/// Generic paged response DTO - demonstrates pagination pattern
/// </summary>
public record PagedResponse<T>
{
    public List<T> Data { get; init; } = new();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

