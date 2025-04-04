using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Cart;
using EasyUiBackend.Api.Extensions;
using AutoMapper;

namespace EasyUiBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;
        private readonly IUIComponentRepository _componentRepository;
        private readonly IMapper _mapper;

        public CartController(
            ICartRepository repository,
            IUIComponentRepository componentRepository,
            IMapper mapper)
        {
            _repository = repository;
            _componentRepository = componentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetUserCart()
        {
            var userId = User.GetUserId();
            var cartItems = await _repository.GetUserCartAsync(userId);
            var cartDtos = _mapper.Map<IEnumerable<CartDto>>(cartItems);
            return Ok(cartDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] CreateCartRequest request)
        {
            var userId = User.GetUserId();
            
            // Check if component exists
            var component = await _componentRepository.GetByIdAsync(request.UIComponentId);
            if (component == null)
                return NotFound("Component not found");

            // Check if item already exists in cart
            var existingItem = await _repository.GetUserCartItemAsync(userId, request.UIComponentId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                await _repository.UpdateAsync(existingItem);
                return Ok(_mapper.Map<CartDto>(existingItem));
            }

            // Create new cart item
            var cart = _mapper.Map<Cart>(request);
            cart.UserId = userId;
            cart.Price = component.Price;

            var result = await _repository.AddAsync(cart);
            return CreatedAtAction(nameof(GetUserCart), _mapper.Map<CartDto>(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(Guid id, [FromBody] UpdateCartRequest request)
        {
            var userId = User.GetUserId();
            var cart = await _repository.GetByIdAsync(id);

            if (cart == null)
                return NotFound();

            if (cart.UserId != userId)
                return Forbid();

            cart.Quantity = request.Quantity;
            await _repository.UpdateAsync(cart);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(Guid id)
        {
            var userId = User.GetUserId();
            var cart = await _repository.GetByIdAsync(id);

            if (cart == null)
                return NotFound();

            if (cart.UserId != userId)
                return Forbid();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
} 