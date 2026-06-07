using EMS.Api.DTOs;
using EMS.Api.Models;
using EMS.Api.Repositories;

namespace EMS.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo) => _repo = repo;

    public async Task<PagedResult<Employee>> GetAllAsync(int page, int pageSize)
    {
        var data = await _repo.GetAllAsync(page, pageSize);
        var total = await _repo.GetCountAsync();
        return new PagedResult<Employee>
        {
            Data = data,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _repo.GetByIdAsync(id);

    public async Task<Employee> CreateAsync(EmployeeDto dto)
    {
        var now = DateTime.UtcNow;
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Department = dto.Department,
            Position = dto.Position,
            Salary = dto.Salary,
            CreatedAt = now,
            UpdatedAt = now
        };
        employee.Id = await _repo.CreateAsync(employee);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(int id, EmployeeDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.Department = dto.Department;
        existing.Position = dto.Position;
        existing.Salary = dto.Salary;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }
        await _repo.DeleteAsync(id);
        return true;
    }
}
