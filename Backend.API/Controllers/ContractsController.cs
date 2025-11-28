using System.Security.Claims;
using Backend.BLL.DTOs.Contract;
using Backend.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IContractAnalysisService _analysisService;

    public ContractsController(IContractAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] CreateContractRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SourceCode))
            return BadRequest("SourceCode is required.");

        // Получаем userId из claims (предположим ClaimTypes.NameIdentifier или "sub")
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null) return Unauthorized();

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            // если в токене лежит не Guid — можно искать в БД по email/username, но для примера — ошибка
            return Unauthorized();
        }

        var result = await _analysisService.AnalyzeAndSaveAsync(userId, request);

        // возвращаем 201 Created с DTO
        return CreatedAtAction(nameof(GetAnalysis), new { id = result.AnalysisId }, result);
    }

    [HttpGet("analysis/{id}")]
    public async Task<IActionResult> GetAnalysis(Guid id)
    {
        try
        {
            var result = await _analysisService.GetAnalysisByIdAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }    
    }
    
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetAnalysesByUserId(Guid userId)
    {
        var analyses = await _analysisService.GetAnalysesByUserIdAsync(userId);

        if (analyses == null || !analyses.Any())
            return NotFound("Анализы для данного пользователя не найдены.");

        return Ok(analyses);
    }
    
}