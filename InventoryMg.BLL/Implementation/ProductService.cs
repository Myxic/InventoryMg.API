using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepo;
        private readonly UserManager<UserProfile> _userManager;

        public ProductService(IUnitOfWork unitOfWork, UserManager<UserProfile> userManager)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.GetRepository<Product>();
            _userManager = userManager;
        }
        public async Task<IEnumerable<ProductView>> GetAllUserProducts(string id)
        {
         UserProfile user = await  _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");
            var products = _productRepo.GetQueryable(p => p.UserId.ToString() == id);
            return products.Select(p => new ProductView
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description ?? "No Descriptions yet",
                Quantity = p.Quantity,
                Category = p.Category,
                BrandName = p.BrandName,
                UserId = p.UserId
            }); ;
           
        }
    }
}
