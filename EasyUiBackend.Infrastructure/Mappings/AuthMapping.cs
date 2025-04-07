using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Auth;

namespace EasyUiBackend.Infrastructure.Mappings
{
    public class AuthMapping : Profile
    {
        public AuthMapping()
        {
            CreateMap<ApplicationUser, UserProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
} 