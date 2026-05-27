using StudentApi.DTOs;
using StudentApi.Models;
using StudentApi.Repositories.Interfaces;
using StudentApi.Services.Interfaces;

namespace StudentApi.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<StudentResponseDto>> GetAllAsync(StudentQueryDto query)
    {
        var result = await _repository.GetAllAsync(query);
        return new PagedResult<StudentResponseDto>
        {
            Data = result.Data.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<StudentResponseDto?> GetByIdAsync(Guid id)
    {
        var student = await _repository.GetByIdAsync(id);
        return student == null ? null : MapToDto(student);
    }

    public async Task<(StudentResponseDto? Student, string? Error)> CreateAsync(CreateStudentDto dto)
    {
        if (await _repository.EmailExistsAsync(dto.Email))
            return (null, "Email already exists.");

        if (await _repository.PhoneExistsAsync(dto.PhoneNumber))
            return (null, "Phone number already exists.");

        var student = new Student
        {
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password) // hashed!
        };

        var created = await _repository.CreateAsync(student);
        return (MapToDto(created), null);
    }

    public async Task<(StudentResponseDto? Student, string? Error)> UpdateAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null) return (null, "Student not found.");

        if (await _repository.EmailExistsAsync(dto.Email, id))
            return (null, "Email already exists.");

        if (await _repository.PhoneExistsAsync(dto.PhoneNumber, id))
            return (null, "Phone number already exists.");

        student.Name = dto.Name;
        student.PhoneNumber = dto.PhoneNumber;
        student.Email = dto.Email;
        student.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var updated = await _repository.UpdateAsync(student);
        return (MapToDto(updated), null);
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _repository.DeleteAsync(id);

    private static StudentResponseDto MapToDto(Student s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        PhoneNumber = s.PhoneNumber,
        Email = s.Email,
        CreatedDate = s.CreatedDate
    };
}