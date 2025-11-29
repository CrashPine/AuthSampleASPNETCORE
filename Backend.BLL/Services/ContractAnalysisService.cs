using AutoMapper;
using Backend.BLL.AI;
using Backend.BLL.DTOs.Contract;
using Backend.BLL.Factory;
using Backend.BLL.Interfaces;
using Backend.DAL;
using Backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.BLL.Services;

public class ContractAnalysisService : IContractAnalysisService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ContractAnalysisPipeline _pipeline;

    // Теперь фабрика используется для получения AiInitializer
    public ContractAnalysisService(
        AppDbContext db,
        IMapper mapper,
        IAiInitializerFactory aiFactory)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        // Получаем модели через фабрику
        var model1 = aiFactory.CreateLocal();
        var model2 = aiFactory.CreateCloud();

        _pipeline = new ContractAnalysisPipeline(model1, model2);
    }

    public async Task<ContractAnalysisDto> AnalyzeAndSaveAsync(Guid userId, CreateContractRequestDto request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // 1) Сохраняем контракт
        var contract = new Contract
        {
            Name = string.IsNullOrWhiteSpace(request.Name) ? "Unnamed" : request.Name,
            SourceCode = request.SourceCode,
            CreatedAt = DateTime.UtcNow
        };

        _db.Contracts.Add(contract);
        await _db.SaveChangesAsync();

        // 2) Анализируем через pipeline
        var finalReport = await _pipeline.AnalyzeContractAsync(request.SourceCode);

        // 3) Сохраняем ContractAnalysis
        var analysis = new ContractAnalysis
        {
            UserId = userId,
            ContractId = contract.Id,
            Summary = finalReport,
            CreatedAt = DateTime.UtcNow
        };

        _db.ContractAnalyses.Add(analysis);
        await _db.SaveChangesAsync();

        // 4) Возвращаем DTO
        var dto = new ContractAnalysisDto
        {
            AnalysisId = analysis.Id,
            ContractId = contract.Id,
            ContractName = contract.Name,
            Summary = analysis.Summary,
            CreatedAt = analysis.CreatedAt,
            SourceCode = contract.SourceCode
            
        };

        return dto;
    }
    
    public async Task<ContractAnalysisDto> GetAnalysisByIdAsync(Guid id)
    {
        var analysis = await _db.ContractAnalyses
            .AsNoTracking()
            .Include(a => a.Contract)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (analysis == null)
            throw new KeyNotFoundException($"Contract analysis with id {id} not found.");

        return new ContractAnalysisDto
        {
            AnalysisId = analysis.Id,
            ContractId = analysis.ContractId,
            ContractName = analysis.Contract?.Name ?? string.Empty,
            Summary = analysis.Summary,
            CreatedAt = analysis.CreatedAt,
            SourceCode = analysis.Contract?.SourceCode ?? string.Empty
        };
    }

    
    public async Task<IEnumerable<ContractAnalysisDto>> GetAnalysesByUserIdAsync(Guid userId)
    {
        var analyses = await _db.ContractAnalyses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Include(a => a.Contract)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return analyses.Select(a => new ContractAnalysisDto
        {
            AnalysisId = a.Id,
            ContractId = a.ContractId,
            ContractName = a.Contract?.Name ?? string.Empty,
            Summary = a.Summary,
            CreatedAt = a.CreatedAt,
            SourceCode = a.Contract?.SourceCode ?? string.Empty
        });

    }
    
    
}