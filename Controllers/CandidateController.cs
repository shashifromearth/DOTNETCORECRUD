using InterviewAPI.DTOs;
using InterviewAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAPI.Controllers;

/// <summary>
/// Candidate Controller - demonstrates RESTful API design
/// Implements full CRUD operations with proper HTTP verbs
/// 
/// Why ControllerBase?
/// - ControllerBase is for Web APIs (no view support needed)
/// - Controller (full MVC) includes View(), ViewBag, etc. which we don't need
/// - ControllerBase provides Ok(), BadRequest(), NotFound(), etc. helper methods
/// 
/// Why [ApiController] attribute?
/// - Enables automatic model validation (returns 400 if invalid)
/// - Automatic binding source inference ([FromBody] inferred for complex types)
/// - Problem details for error responses
/// - Better OpenAPI/Swagger documentation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _candidateService;
    private readonly ILogger<CandidateController> _logger;

    public CandidateController(ICandidateService candidateService, ILogger<CandidateController> logger)
    {
        _candidateService = candidateService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/candidates - Get all candidates with pagination and sorting
    /// Demonstrates: Query parameters, pagination, sorting
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CandidateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CandidateResponse>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc")
    {
        _logger.LogInformation("Getting all candidates - Page: {Page}, Size: {Size}", pageNumber, pageSize);
        
        var result = await _candidateService.GetAllAsync(pageNumber, pageSize, sortBy, sortOrder);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/candidates/{id} - Get candidate by ID
    /// Demonstrates: Route parameters, async/await pattern
    /// 
    /// Why ActionResult&lt;T&gt; instead of IActionResult?
    /// - ActionResult&lt;T&gt; provides type safety and better Swagger documentation
    /// - The generic type tells Swagger what the 200 response contains
    /// - Still allows returning different status codes (Ok, NotFound, etc.)
    /// - Better IntelliSense and compile-time checking
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CandidateResponse>> GetById(int id)
    {
        _logger.LogInformation("Getting candidate with ID: {Id}", id);
        
        var candidate = await _candidateService.GetByIdAsync(id);
        return Ok(candidate);
    }

    /// <summary>
    /// POST /api/candidates - Create a new candidate
    /// Demonstrates: POST verb, model binding, validation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CandidateResponse>> Create(CreateCandidateRequest request)
    {
        _logger.LogInformation("Creating new candidate: {Email}", request.Email);
        
        var candidate = await _candidateService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
    }

    /// <summary>
    /// PUT /api/candidates/{id} - Update an existing candidate
    /// Demonstrates: PUT verb for updates, idempotency
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CandidateResponse>> Update(int id, UpdateCandidateRequest request)
    {
        _logger.LogInformation("Updating candidate with ID: {Id}", id);
        
        var candidate = await _candidateService.UpdateAsync(id, request);
        return Ok(candidate);
    }

    /// <summary>
    /// DELETE /api/candidates/{id} - Delete a candidate
    /// Demonstrates: DELETE verb, resource deletion
    /// 
    /// Why IActionResult here instead of ActionResult&lt;T&gt;?
    /// - No content to return (204 No Content)
    /// - ActionResult&lt;void&gt; doesn't exist, so IActionResult is appropriate
    /// - IActionResult is more flexible when return type varies
    /// - Could also use ActionResult without generic for no-content responses
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting candidate with ID: {Id}", id);
        
        await _candidateService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// GET /api/candidates/search?skill={skill} - Search candidates by skill
    /// Demonstrates: Custom search endpoint, query string parameters
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CandidateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CandidateResponse>>> SearchBySkill([FromQuery] string skill)
    {
        _logger.LogInformation("Searching candidates by skill: {Skill}", skill);
        
        var candidates = await _candidateService.SearchBySkillAsync(skill);
        return Ok(candidates);
    }
}

