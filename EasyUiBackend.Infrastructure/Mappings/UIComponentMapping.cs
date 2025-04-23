using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.UIComponent;

namespace EasyUiBackend.Infrastructure.Mappings;

public class UIComponentMapping : Profile
{
	public UIComponentMapping()
	{
		CreateMap<UIComponent, UIComponent>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Html, opt => opt.MapFrom(src => src.Html))
			.ForMember(dest => dest.Css, opt => opt.MapFrom(src => src.Css))
			.ForMember(dest => dest.Js, opt => opt.MapFrom(src => src.Js))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.PreviewUrl, opt => opt.MapFrom(src => src.PreviewUrl))
			.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
			.ForMember(dest => dest.Framework, opt => opt.MapFrom(src => src.Framework))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
			.ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.DiscountPrice))
			.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
			.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
			.ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
			.ForMember(dest => dest.Updater, opt => opt.MapFrom(src => src.Updater));

		CreateMap<CreateUIComponentRequest, UIComponent>()
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
			.ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.DiscountPrice.HasValue ? src.DiscountPrice.Value : 0m));
		CreateMap<UpdateUIComponentRequest, UIComponent>();

		CreateMap<UIComponent, UIComponentDto>()
			.ForMember(dest => dest.Categories, opt => 
				opt.MapFrom(src => src.Categories.Select(c => c.Name)))
			.ForMember(dest => dest.Tags, opt => 
				opt.MapFrom(src => src.Tags.Select(t => t.Name)))
			.ForMember(dest => dest.Comments, opt => 
				opt.MapFrom(src => src.Comments));

		CreateMap<UIComponent, UIComponentListDto>();
	}
}