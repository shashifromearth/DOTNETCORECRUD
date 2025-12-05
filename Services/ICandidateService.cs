using InterviewAPI.DTOs;

namespace InterviewAPI.Services;

/// <summary>
/// Service interface - demonstrates Service Layer pattern
/// </summary>
public interface ICandidateService
{
    Task<CandidateResponse?> GetByIdAsync(int id);
    Task<PagedResponse<CandidateResponse>> GetAllAsync(int pageNumber, int pageSize, string? sortBy, string? sortOrder);
    Task<CandidateResponse> CreateAsync(CreateCandidateRequest request);
    Task<CandidateResponse?> UpdateAsync(int id, UpdateCandidateRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CandidateResponse>> SearchBySkillAsync(string skill);
}

