using AutoMapper;
using Backend.BLL.DTOs.Contract;
using Backend.DAL.Entities;

namespace Backend.BLL.Mappings;

public class ContractMappingProfile : Profile
{
    public ContractMappingProfile()
    {
        CreateMap<ContractAnalysis, ContractAnalysisDto>()
            .ForMember(d => d.AnalysisId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.ContractName, opt => opt.MapFrom(s => s.Contract.Name));
    }
}