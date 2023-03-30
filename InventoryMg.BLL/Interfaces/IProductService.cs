using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductView>> GetAllUserProducts(string id);
       Task<ProductResult> AddProductAsync(ProductViewRequest product);
        Task<ProductView> GetProductById(Guid prodId);

         Task<ProductResult> EditProductAsync(ProductView product);

        Task<ProductResult> DeleteProductAsync(Guid prodId);
        
        Task<string> UploadProductImage(string prodId,IFormFile file);
    }
}
