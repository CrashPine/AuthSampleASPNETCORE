namespace Backend.DAL.Entities;

public class ContractAnalysis
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Владелец анализа
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Контракт, который анализируем
    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    // Итоговый сводный ответ ИИ
    public string Summary { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
