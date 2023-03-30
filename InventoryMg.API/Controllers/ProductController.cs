using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
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
        //[Authorize(Roles = "Customer", Policy = "Department")]
        public async Task<IActionResult> Addproduct([FromBody] ProductViewRequest product)
        {
            ProductResult result = await _productService.AddProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest(result);
            }
            // return Ok(result);
            return StatusCode(201, result);
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
            return BadRequest(response);
        }

        [HttpPut]
        [Route("update-product-by-id")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductView product)
        {
            ProductResult result = await _productService.EditProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("delete-product-by-id")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            Guid prodId = new Guid(id);
            ProductResult result = await _productService.DeleteProductAsync(prodId);
            if (result.Result)
            {
                return NoContent();
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("upload-product-image")]
        public async Task<IActionResult> Upload(IFormFile file, string prodId)
        {

            var result = await _productService.UploadProductImage(prodId, file);
            if (result == null)
            {
                return BadRequest("Unable to upload ");
            }
            return Ok(result);
        }
    }
}
