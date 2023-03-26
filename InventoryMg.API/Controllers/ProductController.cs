using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;

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
            return BadRequest();
        }

        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> Addproduct([FromBody] ProductViewRequest product)
        {
            ProductResult result = await _productService.AddProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("get-product-by-id")]
        public async Task<IActionResult> GetProductById(string id)
        {

            Guid prodId = new Guid(id);
            ProductView response = await _productService.GetProductById(prodId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("update-product-by-id")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductView product)
        {
            ProductResult result = await _productService.EditProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("delete-product-by-id")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            Guid prodId = new Guid(id);
            ProductResult result = await _productService.DeleteProductAsync(prodId);
            if (result.Result)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
