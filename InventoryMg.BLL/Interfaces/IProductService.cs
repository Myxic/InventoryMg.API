using InventoryMg.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductView> GetAllUserProducts(int id);

      /*  Task<(bool successful, string msg)> AddProductAsync(ProductViewModel product);
        Task<ProductViewModel> GetProductById(int id);
        Task<(bool successful, string msg)> EditProductAsync(ProductViewModel product);

        Task<(bool successful, string msg)> DeleteProductAsync(int userId, int prodId);*/
    }
}
