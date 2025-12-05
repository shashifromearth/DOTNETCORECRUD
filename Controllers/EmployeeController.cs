using InterviewAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAPI.Controllers;

/// <summary>
/// Employee Controller - demonstrates simple CRUD operations using in-memory List
/// This is a simplified version without Repository/Service pattern
/// Shows basic CRUD operations directly in the controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    // In-memory storage - demonstrates simple CRUD with List
    private static readonly List<Employee> _employees = new();
    private static int _nextId = 1;
    private static readonly object _lock = new(); // Thread-safety

    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(ILogger<EmployeeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// GET /api/employee - Get all employees
    /// Demonstrates: Simple GET all operation
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Employee>), StatusCodes.Status200OK)]
    public ActionResult<List<Employee>> GetAll()
    {
        _logger.LogInformation("Getting all employees. Count: {Count}", _employees.Count);
        
        lock (_lock)
        {
            return Ok(_employees);
        }
    }

    /// <summary>
    /// GET /api/employee/{id} - Get employee by ID
    /// Demonstrates: GET by ID with LINQ FirstOrDefault
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Employee> GetById(int id)
    {
        _logger.LogInformation("Getting employee with ID: {Id}", id);
        
        lock (_lock)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }
            
            return Ok(employee);
        }
    }

    /// <summary>
    /// POST /api/employee - Create a new employee
    /// Demonstrates: POST operation, adding to List
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Employee> Create(Employee employee)
    {
        _logger.LogInformation("Creating new employee: {Name}", employee.Name);
        
        if (employee == null)
        {
            return BadRequest("Employee data is required");
        }

        // Simple validation
        if (string.IsNullOrWhiteSpace(employee.Name) || string.IsNullOrWhiteSpace(employee.Email))
        {
            return BadRequest("Name and Email are required");
        }

        lock (_lock)
        {
            // Check if email already exists
            if (_employees.Any(e => e.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest($"Employee with email {employee.Email} already exists");
            }

            // Assign ID and set hire date
            employee.Id = _nextId++;
            employee.HireDate = DateTime.UtcNow;
            
            // Add to list
            _employees.Add(employee);
            
            _logger.LogInformation("Employee created with ID: {Id}", employee.Id);
            
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
    }

    /// <summary>
    /// PUT /api/employee/{id} - Update an existing employee
    /// Demonstrates: PUT operation, updating item in List
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Employee> Update(int id, Employee employee)
    {
        _logger.LogInformation("Updating employee with ID: {Id}", id);
        
        if (employee == null)
        {
            return BadRequest("Employee data is required");
        }

        if (id != employee.Id)
        {
            return BadRequest("ID in URL must match ID in body");
        }

        lock (_lock)
        {
            var existingEmployee = _employees.FirstOrDefault(e => e.Id == id);
            
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            // Check if email is being changed and if new email already exists
            if (!existingEmployee.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (_employees.Any(e => e.Id != id && e.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest($"Employee with email {employee.Email} already exists");
                }
            }

            // Update properties (preserve HireDate)
            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.Salary = employee.Salary;
            // HireDate remains unchanged

            _logger.LogInformation("Employee with ID {Id} updated successfully", id);
            
            return Ok(existingEmployee);
        }
    }

    /// <summary>
    /// DELETE /api/employee/{id} - Delete an employee
    /// Demonstrates: DELETE operation, removing from List
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("Deleting employee with ID: {Id}", id);
        
        lock (_lock)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            _employees.Remove(employee);
            
            _logger.LogInformation("Employee with ID {Id} deleted successfully", id);
            
            return NoContent();
        }
    }
}

