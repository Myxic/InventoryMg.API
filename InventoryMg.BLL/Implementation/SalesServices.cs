using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Enums;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using NotImplementedException = InventoryMg.BLL.Exceptions.NotImplementedException;

namespace InventoryMg.BLL.Implementation
{
    public class SalesServices : ISalesServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Sale> _saleRepo;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IRepository<Product> _productRepo;

        public SalesServices(IUnitOfWork unitOfWork, UserManager<UserProfile> userManager)
        {
            _unitOfWork = unitOfWork;
            _saleRepo = unitOfWork.GetRepository<Sale>();
            _userManager = userManager;
            _productRepo = unitOfWork.GetRepository<Product>();
        }
        public async Task<SalesResponseDto> AddSale(SalesRequestDto model)
        {
            UserProfile existingUser = await _userManager.FindByIdAsync(model.UserId);
            if (existingUser == null)
                throw new NotFoundException($"User with id: {model.UserId} not found");

            Product existingProduct = await _productRepo.GetByIdAsync(new Guid(model.ProductId));
            if (existingProduct == null)
                throw new NotFoundException($"Product with id: {model.ProductId} not found");
            if (!(existingProduct.Quantity > model.Quantity))
                throw new BadRequestException("Product quantity is less than sale quantity");

            var newQuntity = existingProduct.Quantity - model.Quantity;

            var newSale = new Sale()
            {
                Name = model.Name,
                Price = model.Price,
                Category = model.Category,
                Quantity = model.Quantity,
                UserId = new Guid(model.UserId),
                ProductId = new Guid(model.ProductId)
            };
            Sale addedSale = await _saleRepo.AddAsync(newSale);
            if (addedSale == null)
                throw new NotImplementedException("Sale was unable to be added");

            existingProduct.Quantity = newQuntity;

            await _productRepo.UpdateAsync(existingProduct);
            return new SalesResponseDto()
            {
                Id = addedSale.Id,
                Name = addedSale.Name,
                Price = addedSale.Price,
                Category = addedSale.Category,
                Quantity = addedSale.Quantity,
                UserId = addedSale.UserId,
                ProductId = addedSale.ProductId,
            };
        }

        public async Task<bool> DeleteSale(Guid userId, Guid saleId)
        {
            var sale = await _saleRepo.GetByIdAsync(saleId);
            if (sale == null)
                throw new NotFoundException($"Sale with id {saleId}  not found");
            if (sale.UserId != userId)
                throw new BadRequestException($"User Id invalid");

            await _saleRepo.DeleteAsync(sale);
            return true;
        }

        public async Task<SalesResponseDto> EditSale(SalesResponseDto model)
        {
            UserProfile user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                throw new NotFoundException($"User with id: {model.UserId} not found");

            Sale userSale = await _saleRepo.GetSingleByAsync(s => s.Id == model.Id);

            if (userSale != null)
            {
                userSale.Name = model.Name;
                userSale.Price = model.Price;
                userSale.Category = model.Category;
                userSale.Quantity = model.Quantity;
                Sale updatedSale = await _saleRepo.UpdateAsync(userSale);
                if (updatedSale != null)
                {
                    return model;
                }
                throw new NotImplementedException("Unbale to update sale");
            }
            throw new NotFoundException($"Sale with id: {model.Id} not found");
        }

        public async Task<SalesResponseDto> GetSaleById(Guid SaleId)
        {
            var sale = await _saleRepo.GetByIdAsync(SaleId);
            if (sale == null)
                throw new NotFoundException("Sale id not found");

            return new SalesResponseDto
            {
                Id = sale.Id,
                ProductId = sale.ProductId,
                UserId = sale.UserId,
                Name = sale.Name,
                Category = sale.Category,
                Price = sale.Price,
                Quantity = sale.Quantity
            };
        }

        public IEnumerable<SalesResponseDto> GetUserSales(Guid UserId)
        {
            // = await  _saleRepo.GetByAsync(s => s.UserId == UserId);
            var sales = _saleRepo.GetQueryable(p => p.UserId == UserId).OrderBy(i => i.Id);
            if (sales == null)
                throw new NotFoundException("User id was incorrect");
            return sales.Select(s => new SalesResponseDto
            {
                Id = s.Id,
                ProductId = s.ProductId,
                UserId = s.UserId,
                Name = s.Name,
                Category = s.Category,
                Price = s.Price,
                Quantity = s.Quantity
            });
        }


    }
}
