using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.AboutUs;

namespace EasyUiBackend.Infrastructure.Mappings;

public class AboutUsMapping : Profile
{
    public AboutUsMapping()
    {
        CreateMap<AboutUs, AboutUs>();

        CreateMap<CreateAboutUsRequest, AboutUs>();
        
        CreateMap<UpdateAboutUsRequest, AboutUs>();
    }
} 