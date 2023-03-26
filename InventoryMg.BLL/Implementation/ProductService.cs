using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using KeyNotFoundException = InventoryMg.BLL.Exceptions.KeyNotFoundException;
using NotImplementedException = InventoryMg.BLL.Exceptions.NotImplementedException;

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
            var userExist = await _userManager.FindByIdAsync(product.UserId.ToString());

            if (userExist == null)
                throw new KeyNotFoundException($"User Id: {product.UserId} does not match with the product");

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

            var createdProduct = await _productRepo.AddAsync(newProd);

            if (createdProduct != null)
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
                    Message = new List<string>() {
                        "Product was Added Successfully"
                        }
                });
            }

            throw new NotImplementedException("Something went wrong,Unable to Add Product");
        }

        public async Task<ProductResult> DeleteProductAsync(Guid prodId)
        {
            Product productToDelete = await _productRepo.GetSingleByAsync(p => p.Id == prodId);
            if (productToDelete == null)
                throw new NotFoundException($"Invalid product id: {prodId}");
            await _productRepo.DeleteAsync(productToDelete);
            return (new ProductResult()
            {
                Result = true,
                Message = new List<string>()
                    {
                        "Product has Deleted successful"
                    },

            });

        }

        public async Task<ProductResult> EditProductAsync(ProductView product)
        {
            UserProfile user = await _userManager.FindByIdAsync(product.UserId.ToString());
            if (user == null)
                throw new NotFoundException($"User with id: {product.UserId} not found");

            Product userProduct = await _productRepo.GetSingleByAsync(p => p.Id == product.Id);

            if (userProduct != null)
            {

                userProduct.Name = product.Name;
                userProduct.Description = product.Description;
                userProduct.Price = product.Price;
                userProduct.Quantity = product.Quantity;
                userProduct.BrandName = product.BrandName;
                userProduct.Category = product.Category;
                Product updatedProduct = await _productRepo.UpdateAsync(userProduct);
                if (updatedProduct != null)
                {
                    return new ProductResult()
                    {
                        Products = new List<ProductView>
                        {
                            product
                        },
                        Result = true,
                        Message = new List<string>()
                        {
                            "Here are your Products"
                        }

                    };
                }
                throw new NotImplementedException("Unbale to update product");
            }
            throw new NotFoundException($"Product with id: {product.Id} not found");
        }

        public async Task<IEnumerable<ProductView>> GetAllUserProducts(string id)
        {
            UserProfile user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");
            var products = _productRepo.GetQueryable(p => p.UserId.ToString() == id);
            if (products == null)
                throw new NotFoundException("Products not found");

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
            if (product == null) throw new NotFoundException("Invalid Id");
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
