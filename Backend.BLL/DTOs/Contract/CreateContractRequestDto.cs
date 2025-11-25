namespace Backend.BLL.DTOs.Contract;

public class CreateContractRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string SourceCode { get; set; } = string.Empty;
}