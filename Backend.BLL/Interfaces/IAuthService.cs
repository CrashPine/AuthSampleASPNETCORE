using Backend.BLL.DTOs.User;

namespace Backend.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserCreateDto dto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
