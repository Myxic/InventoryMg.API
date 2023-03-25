using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) {
            _productService = productService;
        }

        [HttpGet]
        [Route("get-all-user-product")]
        public async Task<IActionResult> GetProducts(Guid UserId)
        {
         IEnumerable<ProductView> products = await _productService.GetAllUserProducts(UserId.ToString());
            if(products == null)
            {
                return Ok(products);
            }
            return BadRequest("No product found");


        }
    }
}
