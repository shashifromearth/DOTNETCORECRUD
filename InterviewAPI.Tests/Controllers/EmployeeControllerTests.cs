using Bogus;
using FluentAssertions;
using InterviewAPI.Controllers;
using InterviewAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InterviewAPI.Tests.Controllers;

/// <summary>
/// Unit tests for EmployeeController
/// Demonstrates: xUnit, Moq, Bogus, FluentAssertions
/// </summary>
public class EmployeeControllerTests : IDisposable
{
    private readonly Mock<ILogger<EmployeeController>> _mockLogger;
    private readonly EmployeeController _controller;
    private readonly Faker<Employee> _employeeFaker;

    public EmployeeControllerTests()
    {
        // Setup Moq for ILogger
        _mockLogger = new Mock<ILogger<EmployeeController>>();
        
        // Create controller instance
        _controller = new EmployeeController(_mockLogger.Object);
        
        // Setup Bogus Faker for generating test data
        _employeeFaker = new Faker<Employee>()
            .RuleFor(e => e.Id, f => 0) // Will be set by controller
            .RuleFor(e => e.Name, f => f.Person.FullName)
            .RuleFor(e => e.Email, f => f.Person.Email)
            .RuleFor(e => e.Department, f => f.Commerce.Department())
            .RuleFor(e => e.Salary, f => f.Finance.Amount(30000, 150000, 2))
            .RuleFor(e => e.HireDate, f => f.Date.Past(5));
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoEmployeesExist()
    {
        // Arrange - Clear any existing employees
        ClearEmployees();

        // Act
        var result = await Task.FromResult(_controller.GetAll());

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var employees = okResult?.Value as List<Employee>;
        employees.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllEmployees_WhenEmployeesExist()
    {
        // Arrange
        ClearEmployees();
        var employees = _employeeFaker.Generate(3);
        
        foreach (var emp in employees)
        {
            emp.Id = 0; // Reset ID
            var createResult = _controller.Create(emp);
            var createdResult = createResult.Result as CreatedAtActionResult;
            var createdEmployee = createdResult?.Value as Employee;
            emp.Id = createdEmployee!.Id;
        }

        // Act
        var result = _controller.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedEmployees = okResult?.Value as List<Employee>;
        returnedEmployees.Should().NotBeNull().And.HaveCount(3);
    }

    [Fact]
    public async Task GetById_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        
        var createResult = _controller.Create(employee);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var createdEmployee = createdResult?.Value as Employee;
        var employeeId = createdEmployee!.Id;

        // Act
        var result = _controller.GetById(employeeId);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedEmployee = okResult?.Value as Employee;
        returnedEmployee.Should().NotBeNull();
        returnedEmployee!.Id.Should().Be(employeeId);
        returnedEmployee.Email.Should().Be(employee.Email);
        returnedEmployee.Name.Should().Be(employee.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        ClearEmployees();
        var nonExistentId = 999;

        // Act
        var result = _controller.GetById(nonExistentId);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult?.Value.Should().Be($"Employee with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedEmployee_WhenValidDataProvided()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;

        // Act
        var result = _controller.Create(employee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult?.StatusCode.Should().Be(201);
        
        var createdEmployee = createdResult?.Value as Employee;
        createdEmployee.Should().NotBeNull();
        createdEmployee!.Id.Should().BeGreaterThan(0);
        createdEmployee.Name.Should().Be(employee.Name);
        createdEmployee.Email.Should().Be(employee.Email);
        createdEmployee.Department.Should().Be(employee.Department);
        createdEmployee.Salary.Should().Be(employee.Salary);
        createdEmployee.HireDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenEmployeeIsNull()
    {
        // Arrange
        ClearEmployees();
        Employee? employee = null;

        // Act
        var result = _controller.Create(employee!);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult?.Value.Should().Be("Employee data is required");
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        employee.Name = string.Empty;

        // Act
        var result = _controller.Create(employee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult?.Value.Should().Be("Name and Email are required");
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        employee.Email = string.Empty;

        // Act
        var result = _controller.Create(employee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        ClearEmployees();
        var employee1 = _employeeFaker.Generate();
        employee1.Id = 0;
        var createResult1 = _controller.Create(employee1);
        
        var employee2 = _employeeFaker.Generate();
        employee2.Id = 0;
        employee2.Email = employee1.Email; // Duplicate email

        // Act
        var result = _controller.Create(employee2);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult?.Value.Should().Be($"Employee with email {employee1.Email} already exists");
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedEmployee_WhenValidDataProvided()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        
        var createResult = _controller.Create(employee);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var createdEmployee = createdResult?.Value as Employee;
        var employeeId = createdEmployee!.Id;

        var updatedEmployee = _employeeFaker.Generate();
        updatedEmployee.Id = employeeId;
        updatedEmployee.Email = "newemail@example.com"; // Different email

        // Act
        var result = _controller.Update(employeeId, updatedEmployee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedEmployee = okResult?.Value as Employee;
        returnedEmployee.Should().NotBeNull();
        returnedEmployee!.Id.Should().Be(employeeId);
        returnedEmployee.Name.Should().Be(updatedEmployee.Name);
        returnedEmployee.Email.Should().Be(updatedEmployee.Email);
        returnedEmployee.Department.Should().Be(updatedEmployee.Department);
        returnedEmployee.Salary.Should().Be(updatedEmployee.Salary);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        ClearEmployees();
        var nonExistentId = 999;
        var employee = _employeeFaker.Generate();
        employee.Id = nonExistentId;

        // Act
        var result = _controller.Update(nonExistentId, employee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult?.Value.Should().Be($"Employee with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        
        var createResult = _controller.Create(employee);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var createdEmployee = createdResult?.Value as Employee;
        var employeeId = createdEmployee!.Id;

        var updatedEmployee = _employeeFaker.Generate();
        updatedEmployee.Id = employeeId + 1; // Different ID

        // Act
        var result = _controller.Update(employeeId, updatedEmployee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult?.Value.Should().Be("ID in URL must match ID in body");
    }

    [Fact]
    public async Task Update_ShouldPreserveHireDate_WhenUpdatingEmployee()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        
        var createResult = _controller.Create(employee);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var createdEmployee = createdResult?.Value as Employee;
        var employeeId = createdEmployee!.Id;
        var originalHireDate = createdEmployee.HireDate;

        var updatedEmployee = _employeeFaker.Generate();
        updatedEmployee.Id = employeeId;
        updatedEmployee.HireDate = DateTime.UtcNow.AddYears(-10); // Try to change hire date

        // Act
        var result = _controller.Update(employeeId, updatedEmployee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedEmployee = okResult?.Value as Employee;
        returnedEmployee!.HireDate.Should().Be(originalHireDate); // HireDate should be preserved
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        ClearEmployees();
        var employee1 = _employeeFaker.Generate();
        employee1.Id = 0;
        var createResult1 = _controller.Create(employee1);
        
        var employee2 = _employeeFaker.Generate();
        employee2.Id = 0;
        var createResult2 = _controller.Create(employee2);
        var createdResult2 = createResult2.Result as CreatedAtActionResult;
        var createdEmployee2 = createdResult2?.Value as Employee;
        var employee2Id = createdEmployee2!.Id;

        // Try to update employee2 with employee1's email
        var updatedEmployee = _employeeFaker.Generate();
        updatedEmployee.Id = employee2Id;
        updatedEmployee.Email = employee1.Email; // Duplicate email

        // Act
        var result = _controller.Update(employee2Id, updatedEmployee);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult?.Value.Should().Be($"Employee with email {employee1.Email} already exists");
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenEmployeeExists()
    {
        // Arrange
        ClearEmployees();
        var employee = _employeeFaker.Generate();
        employee.Id = 0;
        
        var createResult = _controller.Create(employee);
        var createdResult = createResult.Result as CreatedAtActionResult;
        var createdEmployee = createdResult?.Value as Employee;
        var employeeId = createdEmployee!.Id;

        // Act
        var result = _controller.Delete(employeeId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        
        // Verify employee is deleted
        var getResult = _controller.GetById(employeeId);
        getResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        ClearEmployees();
        var nonExistentId = 999;

        // Act
        var result = _controller.Delete(nonExistentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.Value.Should().Be($"Employee with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task Create_ShouldAutoIncrementId_WhenCreatingMultipleEmployees()
    {
        // Arrange
        ClearEmployees();
        var employees = _employeeFaker.Generate(5);
        var createdIds = new List<int>();

        // Act
        foreach (var emp in employees)
        {
            emp.Id = 0;
            var result = _controller.Create(emp);
            var createdResult = result.Result as CreatedAtActionResult;
            var createdEmployee = createdResult?.Value as Employee;
            createdIds.Add(createdEmployee!.Id);
        }

        // Assert
        createdIds.Should().HaveCount(5);
        createdIds.Should().BeInAscendingOrder();
        createdIds.Should().OnlyHaveUniqueItems();
        createdIds.First().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAll_ShouldBeThreadSafe_WhenAccessedConcurrently()
    {
        // Arrange
        ClearEmployees();
        var employees = _employeeFaker.Generate(10);
        
        // Create employees
        foreach (var emp in employees)
        {
            emp.Id = 0;
            _controller.Create(emp);
        }

        // Act - Simulate concurrent access
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => _controller.GetAll()))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All should return same count
        results.Should().AllSatisfy(r =>
        {
            r.Should().NotBeNull();
            r.Result.Should().BeOfType<OkObjectResult>();
            var okResult = r.Result as OkObjectResult;
            var employeesList = okResult?.Value as List<Employee>;
            employeesList.Should().HaveCount(10);
        });
    }

    // Helper method to clear employees for test isolation
    private void ClearEmployees()
    {
        // Use reflection to access private static field
        var field = typeof(EmployeeController).GetField("_employees", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        if (field?.GetValue(null) is List<Employee> employees)
        {
            employees.Clear();
        }

        // Reset next ID
        var idField = typeof(EmployeeController).GetField("_nextId", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        idField?.SetValue(null, 1);
    }

    public void Dispose()
    {
        ClearEmployees();
    }
}

