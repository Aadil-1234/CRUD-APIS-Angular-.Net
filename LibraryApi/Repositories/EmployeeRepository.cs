using LibraryApi.Data;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<PaginatedResult<Employee>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim();
            query = query.Where(e => EF.Functions.ILike(e.EmployeeName, $"%{searchLower}%") || 
                                     EF.Functions.ILike(e.Department, $"%{searchLower}%") ||
                                     EF.Functions.ILike(e.Email, $"%{searchLower}%"));
        }

        query = (sortBy?.ToLower()) switch
        {
            "name-az" => query.OrderBy(e => e.EmployeeName.ToLower()),
            "name-za" => query.OrderByDescending(e => e.EmployeeName.ToLower()),
            "dept-az" => query.OrderBy(e => e.Department.ToLower()),
            "dept-za" => query.OrderByDescending(e => e.Department.ToLower()),
            _ => query.OrderBy(e => e.Id)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Employee>
        {
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize
        };
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
