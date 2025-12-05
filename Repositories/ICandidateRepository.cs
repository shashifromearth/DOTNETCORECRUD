using InterviewAPI.Models;

namespace InterviewAPI.Repositories;

/// <summary>
/// Repository interface - demonstrates Repository Pattern and Interface Segregation
/// </summary>
public interface ICandidateRepository
{
    Task<Candidate?> GetByIdAsync(int id);
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate> CreateAsync(Candidate candidate);
    Task<Candidate?> UpdateAsync(int id, Candidate candidate);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Candidate>> SearchBySkillAsync(string skill);
}

