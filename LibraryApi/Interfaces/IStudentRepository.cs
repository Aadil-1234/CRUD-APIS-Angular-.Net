using LibraryApi.Models;

namespace LibraryApi.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<PaginatedResult<Student>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize);
    Task<Student?> GetByIdAsync(int id);
    Task<Student> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
}
