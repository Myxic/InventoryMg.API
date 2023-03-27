using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISalesServices _salesServices;

        public SaleController(ISalesServices salesServices)
        {
            _salesServices = salesServices;
        }

        [HttpPost]
        [Route("add-sale")]
        public async Task<IActionResult> AddSale(SalesRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _salesServices.AddSale(model);
            if (response != null)
            {
                return Ok(response);
            }

            return BadRequest("something went wrong");
        }

        [HttpGet]
        [Route("get-all-user-sales")]
        public IActionResult GetUserSales(string userId)
        {
            var id = new Guid(userId);
            var sales = _salesServices.GetUserSales(id);
            return Ok(sales);
        }

        [HttpDelete]
        [Route("delete-by-id")]
        public async Task<IActionResult> Delete(string userId, string saleId)
        {
            var result = await _salesServices.DeleteSale(new Guid(userId), new Guid(saleId));
            if (result)
            {
                return Ok("Sale deleted");
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("get-sale-by-id")]
        public async Task<IActionResult> GetSaleById(string id)
        {

            Guid saleId = new Guid(id);
            SalesResponseDto response = await _salesServices.GetSaleById(saleId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("edit-sale-by-id")]
        public async Task<IActionResult> UpdateSale(SalesResponseDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            SalesResponseDto edited = await _salesServices.EditSale(model);
            if (edited == null)
                return BadRequest();

            return Ok(edited);
        }
    }
}
