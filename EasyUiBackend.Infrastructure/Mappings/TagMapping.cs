using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Tag;

namespace EasyUiBackend.Infrastructure.Mappings;

public class TagMapping : Profile
{
    public TagMapping()
    {
        CreateMap<Tag, Tag>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
            .ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.Updater))
            .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components));

        CreateMap<CreateTagRequest, Tag>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<UpdateTagRequest, Tag>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
} 