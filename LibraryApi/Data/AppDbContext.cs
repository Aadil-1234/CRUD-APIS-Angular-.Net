using Microsoft.EntityFrameworkCore;
using LibraryApi.Models; // This tells this file where the 'Book' class is

namespace LibraryApi.Data; // This gives this file a "home address"

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Student> Students { get; set; }
}