namespace Backend.DAL.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигация
    public List<RefreshToken> RefreshTokens { get; set; } = new();

    // История анализов контрактов
    public List<ContractAnalysis> ContractAnalyses { get; set; } = new();
}