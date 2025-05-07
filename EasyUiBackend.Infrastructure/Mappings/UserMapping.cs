using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.User;
using System.Text.Json;
using System.Collections.Generic;

namespace EasyUiBackend.Infrastructure.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // Tạo options cụ thể cho JsonSerializer
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // Add mapping for ApplicationUser to UserSummaryResponse
            CreateMap<ApplicationUser, UserSummaryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));
            
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
                
            // User detail mapping with work history & education JSON deserialization
            CreateMap<ApplicationUser, UserDetailDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.FollowersCount))
                .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.FollowingCount))
                .ForMember(dest => dest.WorkHistory, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.WorkHistory) 
                        ? new List<WorkHistoryItem>() 
                        : JsonSerializer.Deserialize<List<WorkHistoryItem>>(src.WorkHistory, options)))
                .ForMember(dest => dest.Education, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.Education) 
                        ? new List<EducationItem>() 
                        : JsonSerializer.Deserialize<List<EducationItem>>(src.Education, options)));
                        
            // Update profile request mapping
            CreateMap<UpdateUserProfileRequest, ApplicationUser>()
                .ForMember(dest => dest.WorkHistory, opt => opt.MapFrom(src => 
                    src.WorkHistory == null || !src.WorkHistory.Any() 
                        ? null 
                        : JsonSerializer.Serialize(src.WorkHistory, options)))
                .ForMember(dest => dest.Education, opt => opt.MapFrom(src => 
                    src.Education == null || !src.Education.Any() 
                        ? null 
                        : JsonSerializer.Serialize(src.Education, options)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 