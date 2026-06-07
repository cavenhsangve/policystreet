using EMS.Api.DTOs;
using EMS.Api.Models;
using EMS.Api.Repositories;
using EMS.Api.Services;
using Moq;

namespace EMS.Tests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repoMock;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_repoMock.Object);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult_WithCorrectMetadata()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, FirstName = "Ahmad", LastName = "Razali", Email = "ahmad@ems.local" },
            new() { Id = 2, FirstName = "Siti",  LastName = "Norzahra", Email = "siti@ems.local" }
        };
        _repoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(employees);
        _repoMock.Setup(r => r.GetCountAsync()).ReturnsAsync(2);

        // Act
        var result = await _service.GetAllAsync(1, 10);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyData_WhenNoEmployees()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(new List<Employee>());
        _repoMock.Setup(r => r.GetCountAsync()).ReturnsAsync(0);

        // Act
        var result = await _service.GetAllAsync(1, 10);

        // Assert
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Data);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee_WhenExists()
    {
        // Arrange
        var employee = new Employee { Id = 1, FirstName = "Ahmad", Email = "ahmad@ems.local" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Ahmad", result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_MapsDto_AndReturnsEmployeeWithNewId()
    {
        // Arrange
        var dto = new EmployeeDto
        {
            FirstName  = "Bob",
            LastName   = "Tan",
            Email      = "bob@ems.local",
            Phone      = "+60199998888",
            Department = "Engineering",
            Position   = "Developer",
            Salary     = 8500
        };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Employee>())).ReturnsAsync(42);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(42, result.Id);
        Assert.Equal("Bob", result.FirstName);
        Assert.Equal("bob@ems.local", result.Email);
        Assert.Equal(8500, result.Salary);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var dto = new EmployeeDto
        {
            FirstName = "Bob", LastName = "Tan", Email = "bob@ems.local",
            Department = "IT", Position = "Dev", Salary = 5000
        };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Employee>())).ReturnsAsync(1);

        // Act
        var before = DateTime.UtcNow;
        var result = await _service.CreateAsync(dto);
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(result.CreatedAt, before, after);
        Assert.InRange(result.UpdatedAt, before, after);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenEmployeeNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.UpdateAsync(99, new EmployeeDto());

        // Assert
        Assert.Null(result);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields_AndReturnsEmployee()
    {
        // Arrange
        var existing = new Employee
        {
            Id = 1, FirstName = "Old", LastName = "Name",
            Email = "old@ems.local", Department = "HR", Position = "Analyst", Salary = 5000
        };
        var dto = new EmployeeDto
        {
            FirstName = "New", LastName = "Name",
            Email = "new@ems.local", Department = "Engineering", Position = "Lead", Salary = 9000
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("new@ems.local", result.Email);
        Assert.Equal(9000, result.Salary);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEmployeeNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result);
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_AndCallsRepository_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { Id = 1, FirstName = "Ahmad", Email = "ahmad@ems.local" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
