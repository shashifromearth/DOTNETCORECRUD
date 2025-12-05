using InterviewAPI.DTOs;
using InterviewAPI.Exceptions;
using InterviewAPI.Models;
using InterviewAPI.Repositories;

namespace InterviewAPI.Services;

/// <summary>
/// Service implementation - demonstrates Service Layer with business logic
/// Handles mapping between entities and DTOs, validation, and business rules
/// </summary>
public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(ICandidateRepository repository, ILogger<CandidateService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CandidateResponse?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting candidate with ID: {Id}", id);
        var candidate = await _repository.GetByIdAsync(id);
        
        if (candidate == null)
        {
            throw new NotFoundException($"Candidate with ID {id} not found");
        }

        return MapToResponse(candidate);
    }

    public async Task<PagedResponse<CandidateResponse>> GetAllAsync(int pageNumber, int pageSize, string? sortBy, string? sortOrder)
    {
        _logger.LogInformation("Getting all candidates - Page: {Page}, Size: {Size}", pageNumber, pageSize);
        
        var allCandidates = await _repository.GetAllAsync();
        var candidatesList = allCandidates.ToList();

        // Sorting - demonstrates LINQ OrderBy with dynamic sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortOrder?.ToLower() == "desc";
            candidatesList = SortCandidates(candidatesList, sortBy, isDescending).ToList();
        }

        // Pagination - demonstrates pagination logic
        var totalCount = candidatesList.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedData = candidatesList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToResponse)
            .ToList();

        return new PagedResponse<CandidateResponse>
        {
            Data = pagedData,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<CandidateResponse> CreateAsync(CreateCandidateRequest request)
    {
        _logger.LogInformation("Creating new candidate: {Email}", request.Email);
        
        // Business logic validation
        var existingCandidates = await _repository.GetAllAsync();
        if (existingCandidates.Any(c => c.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException($"A candidate with email {request.Email} already exists");
        }

        var candidate = new Candidate
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            ExperienceYears = request.ExperienceYears,
            Skills = request.Skills ?? new List<string>()
        };

        var created = await _repository.CreateAsync(candidate);
        return MapToResponse(created);
    }

    public async Task<CandidateResponse?> UpdateAsync(int id, UpdateCandidateRequest request)
    {
        _logger.LogInformation("Updating candidate with ID: {Id}", id);
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"Candidate with ID {id} not found");
        }

        // Business logic: Check email uniqueness if changed
        if (!existing.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var allCandidates = await _repository.GetAllAsync();
            if (allCandidates.Any(c => c.Id != id && c.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"A candidate with email {request.Email} already exists");
            }
        }

        var candidate = new Candidate
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            ExperienceYears = request.ExperienceYears,
            Skills = request.Skills ?? new List<string>(),
            AppliedDate = existing.AppliedDate
        };

        var updated = await _repository.UpdateAsync(id, candidate);
        return updated != null ? MapToResponse(updated) : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting candidate with ID: {Id}", id);
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"Candidate with ID {id} not found");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CandidateResponse>> SearchBySkillAsync(string skill)
    {
        _logger.LogInformation("Searching candidates by skill: {Skill}", skill);
        
        if (string.IsNullOrWhiteSpace(skill))
        {
            throw new ValidationException("Skill parameter is required");
        }

        var candidates = await _repository.SearchBySkillAsync(skill);
        return candidates.Select(MapToResponse);
    }

    // Private helper method - demonstrates encapsulation
    private static CandidateResponse MapToResponse(Candidate candidate)
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

    // Demonstrates LINQ with dynamic sorting
    private static IEnumerable<Candidate> SortCandidates(List<Candidate> candidates, string sortBy, bool isDescending)
    {
        return sortBy.ToLower() switch
        {
            "firstname" => isDescending ? candidates.OrderByDescending(c => c.FirstName) : candidates.OrderBy(c => c.FirstName),
            "lastname" => isDescending ? candidates.OrderByDescending(c => c.LastName) : candidates.OrderBy(c => c.LastName),
            "email" => isDescending ? candidates.OrderByDescending(c => c.Email) : candidates.OrderBy(c => c.Email),
            "experienceyears" => isDescending ? candidates.OrderByDescending(c => c.ExperienceYears) : candidates.OrderBy(c => c.ExperienceYears),
            "applieddate" => isDescending ? candidates.OrderByDescending(c => c.AppliedDate) : candidates.OrderBy(c => c.AppliedDate),
            _ => candidates
        };
    }
}

