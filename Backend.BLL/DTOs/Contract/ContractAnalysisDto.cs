namespace Backend.BLL.DTOs.Contract;

public class ContractAnalysisDto
{
    public Guid AnalysisId { get; set; }
    public Guid ContractId { get; set; }
    public string ContractName { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}