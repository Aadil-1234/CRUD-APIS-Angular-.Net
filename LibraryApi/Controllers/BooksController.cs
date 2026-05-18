using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;   // Connects to your Data folder
using LibraryApi.Models; // Connects to your Book model

namespace LibraryApi.Controllers; // Home address for this controller

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase 
{
    private readonly AppDbContext _context;

    // Constructor: Injecting the Database Context
    public BooksController(AppDbContext context) 
    {
        _context = context;
    }

    // GET: api/books (Read all, search, sort, and paginate)
    [HttpGet] 
    public async Task<ActionResult<PaginatedResult<Book>>> Get(
        [FromQuery] string? search, 
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10) 
    {
        // Validation for pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Cap page size for performance

        var query = _context.Books.AsQueryable();

        // 1. Robust Search Logic (Title or Author) using PostgreSQL ILIKE
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim();
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{searchLower}%") || 
                                     EF.Functions.ILike(b.Author, $"%{searchLower}%"));
        }

        // 2. Alphabetical Sorting (A-Z or Z-A)
        query = (sortBy?.ToLower()) switch
        {
            "az" => query.OrderBy(b => b.Title.ToLower()),
            "za" => query.OrderByDescending(b => b.Title.ToLower()),
            "author-az" => query.OrderBy(b => b.Author.ToLower()),
            "author-za" => query.OrderByDescending(b => b.Author.ToLower()),
            _ => query.OrderBy(b => b.Id) // Default: Sort by Id
        };

        // 3. Pagination Logic
        var totalCount = await query.CountAsync();
        
        // Validation: If page is out of range, reset to last page or 1
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PaginatedResult<Book>
        {
            Items = items,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    // POST: api/books (Create)
    [HttpPost] 
    public async Task<ActionResult<Book>> Create(Book book) 
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return Ok(book);
    }

    // PUT: api/books/5 (Update)
    [HttpPut("{id}")] 
    public async Task<IActionResult> Update(int id, Book book) 
    {
        if (id != book.Id) return BadRequest();

        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/books/5 (Delete)
    [HttpDelete("{id}")] 
    public async Task<IActionResult> Delete(int id) 
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}