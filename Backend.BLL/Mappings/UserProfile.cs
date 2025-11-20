using AutoMapper;
using Backend.BLL.DTOs.User;
using Backend.DAL.Entities;

namespace Backend.BLL.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
