using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<PagedResult<Student>> GetAllAsync(StudentQueryDto query);
    Task<Student?> GetByIdAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(Guid id);
}