using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.User;

namespace EasyUiBackend.Infrastructure.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // User follow mappings
            CreateMap<UserFollow, UserFollowDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.FollowerId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Follower.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Follower.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Follower.FullName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Follower.Avatar));
                
            // Profile mapping
            CreateMap<ApplicationUser, UserProfileWithFollowsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.FollowersCount))
                .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.FollowingCount));
        }
    }
} 