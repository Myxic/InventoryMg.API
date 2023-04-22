using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("get-all-payment")]
        [SwaggerOperation(Summary = "Get all payment made on the application", Description = "Requires authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a list of payments")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var result = await _paymentService.GetPayments();
            return Ok(result);

        }

        [HttpGet("get-transaction-by-id")]
        [SwaggerOperation(Summary = "Get Payment by id", Description = "Requires authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a single payment")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> GetTransactionById(string id)
        {
            var result = await _paymentService.GetPaymentByid(id);
            if (result == null)
            {
                return NotFound($"Transaction with id: {id} not found");
            }
            return Ok(result);
        }

        [HttpPost("user-make-payment")]
        [SwaggerOperation(Summary = "Create Payment", Description = "Requires authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return a Transaction Initialize Response")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> MakePayment(PaymentRequest request)
        {
            var result = await _paymentService.InitalizePayment(request);
            if (result == null)
            {
                return BadRequest("Payment was unable to be completed");
            }
            return Ok(result);
        }

        [HttpPut("verify-payment")]
        [SwaggerOperation(Summary = "Verify Payment by id", Description = "Requires authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return a Transaction Verify Response")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            var result = await _paymentService.VerifyPayment(reference);
            if (result == null)
            {
                return BadRequest("Payment verification was unable to be completed");
            }
            return Ok(result);

        }
    }
}
