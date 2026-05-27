using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services.Interfaces;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // GET api/students?search=john&orderBy=name&orderDirection=asc&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] StudentQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(result);
    }

    // GET api/students/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null) return NotFound(new { message = "Student not found." });
        return Ok(student);
    }

    // POST api/students
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
    {
        var (student, error) = await _service.CreateAsync(dto);
        if (error != null) return Conflict(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = student!.Id }, student);
    }

    // PUT api/students/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentDto dto)
    {
        var (student, error) = await _service.UpdateAsync(id, dto);
        if (error == "Student not found.") return NotFound(new { message = error });
        if (error != null) return Conflict(new { message = error });
        return Ok(student);
    }

    // DELETE api/students/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = "Student not found." });
        return NoContent();
    }
}