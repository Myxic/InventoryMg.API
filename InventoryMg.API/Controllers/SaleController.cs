using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SaleController : ControllerBase
    {
        private readonly ISalesServices _salesServices;

        public SaleController(ISalesServices salesServices)
        {
            _salesServices = salesServices;
        }

        [HttpPost("add-sale")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Create a new Sale", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return the newly crated sale")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> AddSale([FromBody] SalesRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _salesServices.AddSale(model);
            if (response != null)
            {
                return StatusCode(201,response);
            }

            return BadRequest("something went wrong");
        }

        [HttpGet("get-all-user-sales")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Get all user Sale", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return the newly crated Sale")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No sale Avalible")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public IActionResult GetUserSales()
        {
           
            var sales = _salesServices.GetUserSales();
            return Ok(sales);
        }

        [HttpDelete("delete-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Delete Sale", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Return no content")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> Delete(string saleId)
        {
            var result = await _salesServices.DeleteSale(new Guid(saleId));
            if (result)
            {
                return Ok("Sale deleted");
            }
            return BadRequest(new {message ="Unable to delete", status = result});
        }

        [HttpGet("get-sale-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Get Sale by Id", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return the sale")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetSaleById(string id)
        {

            Guid saleId = new Guid(id);
            SalesResponseDto response = await _salesServices.GetSaleById(saleId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("edit-sale-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Update Sale by Id", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return the updated sale")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateSale([FromBody] SalesResponseDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SalesResponseDto edited = await _salesServices.EditSale(model);
            if (edited == null)
                return BadRequest(edited);

            return Ok(edited);
        }
    }
}
