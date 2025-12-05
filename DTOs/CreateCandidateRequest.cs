using System.ComponentModel.DataAnnotations;

namespace InterviewAPI.DTOs;

/// <summary>
/// Request DTO for creating a candidate - demonstrates DTO pattern and DataAnnotations validation
/// </summary>
public record CreateCandidateRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string LastName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone format")]
    public string Phone { get; init; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
    public int ExperienceYears { get; init; }

    public List<string> Skills { get; init; } = new();
}

