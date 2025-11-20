using Backend.BLL.Interfaces;
using Backend.DAL;
using AutoMapper;
using Backend.BLL.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace Backend.BLL.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UserService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserAsync(Guid id, UserUpdateDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) throw new InvalidOperationException("User not found.");

        user.UserName = dto.UserName;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _db.SaveChangesAsync();
    }
}