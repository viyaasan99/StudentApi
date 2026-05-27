using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;
using StudentApi.Repositories.Interfaces;

namespace StudentApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Student>> GetAllAsync(StudentQueryDto query)
    {
        var students = _context.Students.AsQueryable();

        // Search by name, email, or phone in one query
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            students = students.Where(s =>
                s.Name.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search) ||
                s.PhoneNumber.Contains(search));
        }

        // Ordering
        students = query.OrderBy?.ToLower() switch
        {
            "name" => query.OrderDirection?.ToLower() == "desc"
                ? students.OrderByDescending(s => s.Name)
                : students.OrderBy(s => s.Name),
            "createddate" => query.OrderDirection?.ToLower() == "desc"
                ? students.OrderByDescending(s => s.CreatedDate)
                : students.OrderBy(s => s.CreatedDate),
            _ => students.OrderBy(s => s.CreatedDate) // default
        };

        var totalCount = await students.CountAsync();

        var data = await students
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Student>
        {
            Data = data,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Student?> GetByIdAsync(Guid id) =>
        await _context.Students.FindAsync(id);

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null) =>
        await _context.Students.AnyAsync(s => s.Email == email && s.Id != excludeId);

    public async Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null) =>
        await _context.Students.AnyAsync(s => s.PhoneNumber == phone && s.Id != excludeId);

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}