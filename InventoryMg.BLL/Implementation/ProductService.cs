using InventoryMg.BLL.DTOs.Request;
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

        public async Task<ProductResult> AddProductAsync(ProductViewRequest product)
        {
            try
            {
                /* User user = await _userRepo.GetSingleByAsync(u => u.Id == product.UserId, tracking: true);*/
          var userExist = await  _userManager.FindByIdAsync(product.UserId.ToString());

                if (userExist == null)
                {
                    return (new ProductResult()
                    {
                        Products = null,
                        Result = false,
                        Errors = new List<string>()
                        {
                            "User Not Found"
                        }
                    });
                }


                var newProd = new Product
                {
                    UserId = product.UserId,
                    Name = product.Name,
                    Description = product.Description,
                    Category = product.Category,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    BrandName = product.BrandName
                };
               
          var createdProduct =  await _productRepo.AddAsync(newProd);
           
                if(createdProduct != null)
                {
                    return (new ProductResult()
                    {
                        Products = new List<ProductView>()
                        {
                            new ProductView
                            {
                                Name = createdProduct.Name,
                                Description = createdProduct.Description ?? "No Description Yet",
                                Category = createdProduct.Category,
                                Price = createdProduct.Price,
                                BrandName = createdProduct.BrandName,
                                UserId = createdProduct.UserId,
                                Id =  createdProduct.Id
                            }
                        },
                        Result = true,
                        Errors = null
                    });
                }

                return (new ProductResult()
                {
                    Products = null,
                    Result = false,
                    Errors = new List<string>()
                        {
                            "Something went wrong,Unable to Add Product"
                        }
                }); ;
            }
            catch (Exception ex)
            {
               return (new ProductResult()
                {
                    Products = null,
                    Result = false,
                    Errors = new List<string>()
                        {
                            $"{ex.Message}"
                        }
                });
            }
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

        public async Task<ProductView> GetProductById(Guid prodId)
        {

            Product product = await _productRepo.GetByIdAsync(prodId);
            return new ProductView
            {
                Id = product.Id,
                UserId = product.UserId,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Quantity = product.Quantity,
                Price = product.Price,
                BrandName = product.BrandName
            };
        }
    }
}
