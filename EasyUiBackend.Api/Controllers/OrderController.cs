using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Order;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Api.Extensions;
using AutoMapper;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public OrderController(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = User.GetUserId();
        var cartItems = await _cartRepository.GetUserCartAsync(userId);
        
        if (!cartItems.Any())
            return BadRequest("Cart is empty");

        var order = new Order
        {
            UserId = userId,
            PaymentMethod = request.PaymentMethod,
            CreatedBy = userId,
            Items = cartItems.Select(ci => new OrderItem
            {
                UIComponentId = ci.UIComponentId,
                Price = ci.Price,
                Quantity = ci.Quantity,
                Subtotal = ci.Price * ci.Quantity
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.Subtotal);

        var result = await _orderRepository.AddAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, _mapper.Map<OrderDto>(result));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
    {
        var userId = User.GetUserId();
        var orders = await _orderRepository.GetUserOrdersAsync(userId);
        return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        if (order == null)
            return NotFound();

        if (order.UserId != User.GetUserId())
            return Forbid();

        return Ok(_mapper.Map<OrderDto>(order));
    }
} 