using LibraryApi.Data;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<PaginatedResult<Student>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim();
            query = query.Where(s => EF.Functions.ILike(s.Name, $"%{searchLower}%") || 
                                     EF.Functions.ILike(s.Department, $"%{searchLower}%") ||
                                     EF.Functions.ILike(s.Course, $"%{searchLower}%"));
        }

        query = (sortBy?.ToLower()) switch
        {
            "name-az" => query.OrderBy(s => s.Name.ToLower()),
            "name-za" => query.OrderByDescending(s => s.Name.ToLower()),
            "dept-az" => query.OrderBy(s => s.Department.ToLower()),
            "dept-za" => query.OrderByDescending(s => s.Department.ToLower()),
            _ => query.OrderBy(s => s.StudentId)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Student>
        {
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize
        };
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    public async Task<Student> AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
