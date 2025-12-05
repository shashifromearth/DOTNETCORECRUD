namespace InterviewAPI.Models;

/// <summary>
/// Candidate entity model - demonstrates domain modeling
/// </summary>
public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public List<string> Skills { get; set; } = new();
    public DateTime AppliedDate { get; set; }
}

