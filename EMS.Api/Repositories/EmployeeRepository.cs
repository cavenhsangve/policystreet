using Dapper;
using EMS.Api.Exceptions;
using EMS.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EMS.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnection _db;

    public EmployeeRepository(IDbConnection db) => _db = db;

    public async Task<IEnumerable<Employee>> GetAllAsync(int page, int rows)
    {
        const string sql = @"
            SELECT * FROM employees
            WHERE is_deleted = 0
            ORDER BY last_name, first_name
            OFFSET (@Page - 1) * @Rows ROWS
            FETCH NEXT @Rows ROWS ONLY";
        return await _db.QueryAsync<Employee>(sql, new { Page = page, Rows = rows });
    }

    public async Task<int> GetCountAsync()
    {
        const string sql = "SELECT COUNT(*) FROM employees WHERE is_deleted = 0";
        return await _db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT * FROM employees
            WHERE id = @Id AND is_deleted = 0";
        return await _db.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Employee employee)
    {
        try
        {
            const string sql = @"
            INSERT INTO employees (first_name, last_name, email, phone, department, position, salary, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@FirstName, @LastName, @Email, @Phone, @Department, @Position, @Salary, @CreatedAt, @UpdatedAt)";
            return await _db.ExecuteScalarAsync<int>(sql, employee);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            throw new DuplicateEmailException();
        }
    }

    public async Task<int> UpdateAsync(Employee employee)
    {
        try
        {
            const string sql = @"
              UPDATE employees SET
                  first_name  = @FirstName,
                  last_name   = @LastName,
                  email       = @Email,
                  phone       = @Phone,
                  department  = @Department,
                  position    = @Position,
                  salary      = @Salary,
                  updated_at  = @UpdatedAt
              WHERE id = @Id";
            return await _db.ExecuteAsync(sql, employee);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            throw new DuplicateEmailException();
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        const string sql = @"
            UPDATE employees SET 
                is_deleted = 1, 
                updated_at = @UpdatedAt
            WHERE id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
    }
}
