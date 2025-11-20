namespace Backend.BLL.DTOs.User;

public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
