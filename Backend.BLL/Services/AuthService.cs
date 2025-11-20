using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Backend.BLL.DTOs.User;
using Backend.BLL.Interfaces;
using Backend.DAL;
using Backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.BLL.Services;

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings _jwt;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext db, IOptions<JwtSettings> jwtOptions, IMapper mapper)
    {
        _db = db;
        _jwt = jwtOptions.Value;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(UserCreateDto dto)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) throw new InvalidOperationException("User with this email already exists.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return await GenerateTokensForUserAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _db.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) throw new InvalidOperationException("Invalid credentials.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid credentials.");

        return await GenerateTokensForUserAsync(user);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _db.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            return null;

        tokenEntity.IsRevoked = true;
        await _db.SaveChangesAsync();

        return await GenerateTokensForUserAsync(tokenEntity.User);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (tokenEntity == null) return;
        tokenEntity.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokensForUserAsync(User user)
    {
        var accessExpires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);
        var key = Encoding.UTF8.GetBytes(_jwt.Secret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // <-- исправлено
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("username", user.UserName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessExpires,
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N") + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays),
            UserId = user.Id
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshToken.Token, accessExpires);
    }
}
