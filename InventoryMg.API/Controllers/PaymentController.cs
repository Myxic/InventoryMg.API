using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route("get-all-payment")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var result = await _paymentService.GetPayments();
            return Ok(result);

        }

        [HttpGet]
        [Route("get-transaction-by-id")]
        public async Task<IActionResult> GetTransactionById(string id)
        {
            var result = await _paymentService.GetPaymentByid(id);
            if (result == null)
            {
                return NotFound($"Transaction with id: {id} not found");
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("user-make-payment")]
        public async Task<IActionResult> MakePayment(PaymentRequest request)
        {
            var result = await _paymentService.InitalizePayment(request);
            if (result == null)
            {
                return BadRequest("Payment was unable to be completed");
            }
            return Ok(result);
        }

        [HttpPut]
        [Route("verify-payment")]
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
