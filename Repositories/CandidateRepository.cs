using InterviewAPI.Models;

namespace InterviewAPI.Repositories;

/// <summary>
/// Repository implementation - demonstrates Repository Pattern with in-memory storage
/// Uses Dictionary for O(1) lookup and List for iteration
/// </summary>
public class CandidateRepository : ICandidateRepository
{
    // In-memory storage - demonstrates no external database dependency
    private readonly Dictionary<int, Candidate> _candidates = new();
    private int _nextId = 1;
    private readonly object _lock = new(); // Thread-safety for concurrent access

    public Task<Candidate?> GetByIdAsync(int id)
    {
        // Simulate async operation
        lock (_lock)
        {
            _candidates.TryGetValue(id, out var candidate);
            return Task.FromResult(candidate);
        }
    }

    public Task<IEnumerable<Candidate>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Candidate>>(_candidates.Values.ToList());
        }
    }

    public Task<Candidate> CreateAsync(Candidate candidate)
    {
        lock (_lock)
        {
            candidate.Id = _nextId++;
            candidate.AppliedDate = DateTime.UtcNow;
            _candidates[candidate.Id] = candidate;
            return Task.FromResult(candidate);
        }
    }

    public Task<Candidate?> UpdateAsync(int id, Candidate candidate)
    {
        lock (_lock)
        {
            if (!_candidates.ContainsKey(id))
            {
                return Task.FromResult<Candidate?>(null);
            }

            candidate.Id = id;
            candidate.AppliedDate = _candidates[id].AppliedDate; // Preserve original date
            _candidates[id] = candidate;
            return Task.FromResult<Candidate?>(candidate);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            return Task.FromResult(_candidates.Remove(id));
        }
    }

    public Task<IEnumerable<Candidate>> SearchBySkillAsync(string skill)
    {
        lock (_lock)
        {
            // LINQ operation - demonstrates LINQ querying
            var results = _candidates.Values
                .Where(c => c.Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return Task.FromResult<IEnumerable<Candidate>>(results);
        }
    }
}

