namespace InterviewAPI.DTOs;

/// <summary>
/// Response DTO for candidate - demonstrates response DTO pattern using record types
/// </summary>
public record CandidateResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public int ExperienceYears { get; init; }
    public List<string> Skills { get; init; } = new();
    public DateTime AppliedDate { get; init; }
}

