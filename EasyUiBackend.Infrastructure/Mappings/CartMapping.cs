using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Cart;

namespace EasyUiBackend.Infrastructure.Mappings
{
    public class CartMapping : Profile
    {
        public CartMapping()
        {
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.UIComponentName, opt => opt.MapFrom(src => src.UIComponent.Name));
            CreateMap<CreateCartRequest, Cart>();
            CreateMap<UpdateCartRequest, Cart>();
        }
    }
} 