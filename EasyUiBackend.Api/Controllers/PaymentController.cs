using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Payment;
using Microsoft.Extensions.Logging;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("momo/create")]
    public async Task<ActionResult<string>> CreateMomoPayment([FromBody] CreateMomoPaymentRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (request.OrderId == Guid.Empty)
            {
                return BadRequest("Invalid OrderId");
            }

            if (string.IsNullOrEmpty(request.ReturnUrl))
            {
                return BadRequest("ReturnUrl is required");
            }

            var paymentUrl = await _paymentService.CreateMomoPaymentAsync(request);
            return Ok(new { paymentUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MoMo payment");
            return StatusCode(500, new { message = $"MoMo error: {ex.Message}" });
        }
    }

    [AllowAnonymous]
    [HttpPost("momo/ipn")]
    public async Task<IActionResult> MomoIPN([FromBody] Dictionary<string, string> callbackData)
    {
        var success = await _paymentService.ProcessMomoCallbackAsync(callbackData);
        return Ok(new { success });
    }

    [AllowAnonymous]
    [HttpGet("momo/return")]
    public IActionResult MomoReturn([FromQuery] string orderId, [FromQuery] string resultCode)
    {
        var success = resultCode == "0";
        return Redirect(success ? "/payment/success" : "/payment/failure");
    }

    [HttpGet("status/{orderId}")]
    public async Task<ActionResult<string>> GetPaymentStatus(Guid orderId)
    {
        var status = await _paymentService.GetPaymentStatusAsync(orderId);
        return Ok(new { status });
    }
} 