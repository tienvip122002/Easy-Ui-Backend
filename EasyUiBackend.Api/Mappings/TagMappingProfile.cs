using AutoMapper;
using EasyUiBackend.Domain.Models.Tag;
using EasyUiBackend.Contracts.Models.Tag;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>();
    }
} 