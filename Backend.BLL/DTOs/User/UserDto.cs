namespace Backend.BLL.DTOs.User;

public record UserDto(Guid Id, string Email, string UserName, DateTime CreatedAt);