using LibraryApi.Data;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<PaginatedResult<Client>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim();
            query = query.Where(c => EF.Functions.ILike(c.ClientName, $"%{searchLower}%") || 
                                     EF.Functions.ILike(c.Department, $"%{searchLower}%") ||
                                     EF.Functions.ILike(c.Email, $"%{searchLower}%"));
        }

        query = (sortBy?.ToLower()) switch
        {
            "name-az" => query.OrderBy(c => c.ClientName.ToLower()),
            "name-za" => query.OrderByDescending(c => c.ClientName.ToLower()),
            "dept-az" => query.OrderBy(c => c.Department.ToLower()),
            "dept-za" => query.OrderByDescending(c => c.Department.ToLower()),
            _ => query.OrderBy(c => c.ClientId)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Client>
        {
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize
        };
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients.FindAsync(id);
    }

    public async Task<Client> AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Entry(client).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}
