using EMS.Api.Models;

namespace EMS.Api.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(int page, int rows);
    Task<int> GetCountAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<int> CreateAsync(Employee employee);
    Task<int> UpdateAsync(Employee employee);
    Task<int> DeleteAsync(int id);
}
