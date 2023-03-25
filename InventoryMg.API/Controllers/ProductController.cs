using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("get-all-user-product")]
        public async Task<IActionResult> GetProducts(string UserId)
        {
            IEnumerable<ProductView> products = await _productService.GetAllUserProducts(UserId.ToString());
            if (products != null)
            {
                return Ok(products);
            }
            return BadRequest("No product found");
        }

        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> Addproduct([FromBody] ProductViewRequest product)
        {
            ProductResult result = await _productService.AddProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("get-product-by-id")]
        public async Task<IActionResult> GetProductById(string id)
        {

            Guid prodId = new Guid(id);
         ProductView response =  await  _productService.GetProductById(prodId);
            if(response != null)
            {
                return Ok(response);
            }
            return BadRequest("Product not found");
        }
    }
}
