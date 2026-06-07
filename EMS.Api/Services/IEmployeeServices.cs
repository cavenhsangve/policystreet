using EMS.Api.DTOs;
using EMS.Api.Models;

namespace EMS.Api.Services;

public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetAllAsync(int page, int pageSize);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(EmployeeDto dto);
    Task<Employee?> UpdateAsync(int id, EmployeeDto dto);
    Task<bool> DeleteAsync(int id);
}