using LibraryApi.Models;

namespace LibraryApi.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync();
    Task<PaginatedResult<Client>> GetPaginatedAsync(string? search, string? sortBy, int page, int pageSize);
    Task<Client?> GetByIdAsync(int id);
    Task<Client> AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int id);
}
