using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryMg.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all-user-product")]
        [SwaggerOperation(Summary = "Get all product list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return all products")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetProducts()
        {
            IEnumerable<ProductView> products = await _productService.GetAllUserProducts();
            if (products != null)
            {
                return Ok(products);
            }
            return BadRequest();
        }

        [HttpPost("add-product")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Create a new product", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return the newly crated product")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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

        [HttpGet("get-product-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Get product by id", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return the product")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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

        [HttpPut("update-product-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Update product by id", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return the product")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductView product)
        {
            ProductResult result = await _productService.EditProductAsync(product);
            if (result.Result == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete-product-by-id")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Delete product by id", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Return no content")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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

        [HttpPost("upload-product-image")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Update productimage", Description = "Requires cusomer authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return Message")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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
