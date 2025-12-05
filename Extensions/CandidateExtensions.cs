using InterviewAPI.DTOs;
using InterviewAPI.Models;

namespace InterviewAPI.Extensions;

/// <summary>
/// Extension methods - demonstrates C# extension methods pattern
/// Provides utility methods for Candidate operations
/// </summary>
public static class CandidateExtensions
{
    /// <summary>
    /// Extension method to convert Candidate to CandidateResponse
    /// </summary>
    public static CandidateResponse ToResponse(this Candidate candidate)
    {
        return new CandidateResponse
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            ExperienceYears = candidate.ExperienceYears,
            Skills = candidate.Skills,
            AppliedDate = candidate.AppliedDate
        };
    }

    /// <summary>
    /// Extension method to get full name
    /// </summary>
    public static string GetFullName(this Candidate candidate)
    {
        return $"{candidate.FirstName} {candidate.LastName}";
    }

    /// <summary>
    /// Extension method to check if candidate is senior (5+ years experience)
    /// </summary>
    public static bool IsSenior(this Candidate candidate)
    {
        return candidate.ExperienceYears >= 5;
    }
}

