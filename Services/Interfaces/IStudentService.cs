using StudentApi.DTOs;

namespace StudentApi.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentResponseDto>> GetAllAsync(StudentQueryDto query);
    Task<StudentResponseDto?> GetByIdAsync(Guid id);
    Task<(StudentResponseDto? Student, string? Error)> CreateAsync(CreateStudentDto dto);
    Task<(StudentResponseDto? Student, string? Error)> UpdateAsync(Guid id, UpdateStudentDto dto);
    Task<bool> DeleteAsync(Guid id);
}