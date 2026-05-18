using LibraryApi.DTOs;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<EmployeeDto>>> Get(
        [FromQuery] string? search, 
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _repository.GetPaginatedAsync(search, sortBy, page, pageSize);
        
        var employeeDtos = result.Items.Select(e => new EmployeeDto
        {
            Id = e.Id,
            EmployeeName = e.EmployeeName,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email,
            Department = e.Department
        });

        return Ok(new PaginatedResult<EmployeeDto>
        {
            Items = employeeDtos,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null) return NotFound();

        var employeeDto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeName = employee.EmployeeName,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email,
            Department = employee.Department
        };
        return Ok(employeeDto);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto createDto)
    {
        var employee = new Employee
        {
            EmployeeName = createDto.EmployeeName,
            PhoneNumber = createDto.PhoneNumber,
            Email = createDto.Email,
            Department = createDto.Department
        };

        var createdEmployee = await _repository.AddAsync(employee);

        var employeeDto = new EmployeeDto
        {
            Id = createdEmployee.Id,
            EmployeeName = createdEmployee.EmployeeName,
            PhoneNumber = createdEmployee.PhoneNumber,
            Email = createdEmployee.Email,
            Department = createdEmployee.Department
        };

        return CreatedAtAction(nameof(GetById), new { id = employeeDto.Id }, employeeDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EmployeeDto employeeDto)
    {
        if (id != employeeDto.Id) return BadRequest();

        var employee = new Employee
        {
            Id = employeeDto.Id,
            EmployeeName = employeeDto.EmployeeName,
            PhoneNumber = employeeDto.PhoneNumber,
            Email = employeeDto.Email,
            Department = employeeDto.Department
        };

        await _repository.UpdateAsync(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
