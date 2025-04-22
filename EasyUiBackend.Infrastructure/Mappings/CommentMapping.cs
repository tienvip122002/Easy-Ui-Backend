using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Comment;

namespace EasyUiBackend.Infrastructure.Mappings;

public class CommentMapping : Profile
{
    public CommentMapping()
    {
        CreateMap<Comment, Comment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ComponentId, opt => opt.MapFrom(src => src.ComponentId))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
            .ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.Updater))
            .ForMember(dest => dest.Component, opt => opt.MapFrom(src => src.Component));

        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();
        
        // Map Comment entity to CommentDto
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => 
                src.Creator != null ? src.Creator.UserName : null));
    }
} 