using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Payment;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("momo/create")]
    public async Task<ActionResult<string>> CreateMomoPayment([FromBody] CreateMomoPaymentRequest request)
    {
        try
        {
            var paymentUrl = await _paymentService.CreateMomoPaymentAsync(request);
            return Ok(new { paymentUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("momo/return")]
    public async Task<IActionResult> MomoReturn([FromQuery] Dictionary<string, string> callbackData)
    {
        var success = await _paymentService.ProcessMomoCallbackAsync(callbackData);
        return Redirect(success ? "/payment/success" : "/payment/failure");
    }

    [HttpGet("status/{orderId}")]
    public async Task<ActionResult<string>> GetPaymentStatus(Guid orderId)
    {
        var status = await _paymentService.GetPaymentStatusAsync(orderId);
        return Ok(new { status });
    }
} 