using LibraryApi.DTOs;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _repository;

    public ClientsController(IClientRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ClientDto>>> Get(
        [FromQuery] string? search, 
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _repository.GetPaginatedAsync(search, sortBy, page, pageSize);
        
        var clientDtos = result.Items.Select(c => new ClientDto
        {
            ClientId = c.ClientId,
            ClientName = c.ClientName,
            PhoneNumber = c.PhoneNumber,
            Email = c.Email,
            Department = c.Department
        });

        return Ok(new PaginatedResult<ClientDto>
        {
            Items = clientDtos,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client == null) return NotFound();

        var clientDto = new ClientDto
        {
            ClientId = client.ClientId,
            ClientName = client.ClientName,
            PhoneNumber = client.PhoneNumber,
            Email = client.Email,
            Department = client.Department
        };
        return Ok(clientDto);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(ClientCreateDto createDto)
    {
        var client = new Client
        {
            ClientName = createDto.ClientName,
            PhoneNumber = createDto.PhoneNumber,
            Email = createDto.Email,
            Department = createDto.Department
        };

        var createdClient = await _repository.AddAsync(client);

        var clientDto = new ClientDto
        {
            ClientId = createdClient.ClientId,
            ClientName = createdClient.ClientName,
            PhoneNumber = createdClient.PhoneNumber,
            Email = createdClient.Email,
            Department = createdClient.Department
        };

        return CreatedAtAction(nameof(GetById), new { id = clientDto.ClientId }, clientDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClientDto clientDto)
    {
        if (id != clientDto.ClientId) return BadRequest();

        var client = new Client
        {
            ClientId = clientDto.ClientId,
            ClientName = clientDto.ClientName,
            PhoneNumber = clientDto.PhoneNumber,
            Email = clientDto.Email,
            Department = clientDto.Department
        };

        await _repository.UpdateAsync(client);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
