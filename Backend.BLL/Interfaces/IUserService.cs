using Backend.BLL.DTOs.User;

namespace Backend.BLL.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task UpdateUserAsync(Guid id, UserUpdateDto dto);
}
