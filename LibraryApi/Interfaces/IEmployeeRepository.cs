using LibraryApi.Models;

namespace LibraryApi.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<PaginatedResult<Employee>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}
