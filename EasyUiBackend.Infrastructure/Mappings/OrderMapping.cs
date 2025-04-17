using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Order;

namespace EasyUiBackend.Infrastructure.Mappings;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.UIComponentName, opt => opt.MapFrom(src => src.UIComponent.Name));

        CreateMap<Order, PurchasedProductDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Items.First().UIComponent.Name))
            .ForMember(dest => dest.UIComponentId, opt => opt.MapFrom(src => src.Items.First().UIComponentId))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus));
    }
} 