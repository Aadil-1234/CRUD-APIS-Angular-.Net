using LibraryApi.DTOs;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repository;

    public StudentsController(IStudentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<StudentDto>>> Get(
        [FromQuery] string? search, 
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _repository.GetPaginatedAsync(search, sortBy, page, pageSize);
        
        var studentDtos = result.Items.Select(s => new StudentDto
        {
            StudentId = s.StudentId,
            Name = s.Name,
            PhoneNumber = s.PhoneNumber,
            Course = s.Course,
            Department = s.Department
        });

        return Ok(new PaginatedResult<StudentDto>
        {
            Items = studentDtos,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null) return NotFound();

        var studentDto = new StudentDto
        {
            StudentId = student.StudentId,
            Name = student.Name,
            PhoneNumber = student.PhoneNumber,
            Course = student.Course,
            Department = student.Department
        };
        return Ok(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(StudentCreateDto createDto)
    {
        var student = new Student
        {
            Name = createDto.Name,
            PhoneNumber = createDto.PhoneNumber,
            Course = createDto.Course,
            Department = createDto.Department
        };

        var createdStudent = await _repository.AddAsync(student);

        var studentDto = new StudentDto
        {
            StudentId = createdStudent.StudentId,
            Name = createdStudent.Name,
            PhoneNumber = createdStudent.PhoneNumber,
            Course = createdStudent.Course,
            Department = createdStudent.Department
        };

        return CreatedAtAction(nameof(GetById), new { id = studentDto.StudentId }, studentDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StudentDto studentDto)
    {
        if (id != studentDto.StudentId) return BadRequest();

        var student = new Student
        {
            StudentId = studentDto.StudentId,
            Name = studentDto.Name,
            PhoneNumber = studentDto.PhoneNumber,
            Course = studentDto.Course,
            Department = studentDto.Department
        };

        await _repository.UpdateAsync(student);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
