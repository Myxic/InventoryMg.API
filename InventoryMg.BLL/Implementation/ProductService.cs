using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Implementation
{
    public class ProductService : IProductService
    {
        public IEnumerable<ProductView> GetAllUserProducts(int id)
        {
            throw new NotImplementedException();
        }
    }
}
