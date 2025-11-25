namespace Backend.DAL.Entities;

public class Contract
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Основная информация о контракте
    public string Name { get; set; } = null!;
    public string SourceCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Не обязательно хранить анализы здесь, но можно для навигации
    public List<ContractAnalysis> Analyses { get; set; } = new();
}