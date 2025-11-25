using Backend.BLL.DTOs.Contract;

namespace Backend.BLL.Interfaces;

public interface IContractAnalysisService
{
    /// <summary>
    /// Анализирует контракт (SourceCode), сохраняет Contract и ContractAnalysis и возвращает DTO результата.
    /// </summary>
    Task<ContractAnalysisDto> AnalyzeAndSaveAsync(Guid userId, CreateContractRequestDto request);
}