using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NotImplementedException = InventoryMg.BLL.Exceptions.NotImplementedException;

namespace InventoryMg.BLL.Implementation
{
    public class SalesServices : ISalesServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Sale> _saleRepo;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;
        private readonly HttpContextAccessor _httpContextAccessor;


        public SalesServices(IUnitOfWork unitOfWork, UserManager<UserProfile> userManager,
            IMapper mapper, HttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _saleRepo = unitOfWork.GetRepository<Sale>();
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productRepo = unitOfWork.GetRepository<Product>();
        }
        public async Task<SalesResponseDto> AddSale(SalesRequestDto model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("User not logged in");

            UserProfile existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
                throw new NotFoundException($"User with id: {userId} not found");

            Product existingProduct = await _productRepo.GetByIdAsync(new Guid(model.ProductId));
            if (existingProduct == null)
                throw new NotFoundException($"Product with id: {model.ProductId} not found");
            if (!(existingProduct.Quantity > model.Quantity))
                throw new BadRequestException("Product quantity is less than sale quantity");

            var newQuntity = existingProduct.Quantity - model.Quantity;
            var newSale = _mapper.Map<Sale>(model);

            Sale addedSale = await _saleRepo.AddAsync(newSale);
            if (addedSale == null)
                throw new NotImplementedException("Sale was unable to be added");

            existingProduct.Quantity = newQuntity;

            await _productRepo.UpdateAsync(existingProduct);
            return _mapper.Map<SalesResponseDto>(addedSale);
        }

        public async Task<bool> DeleteSale(Guid saleId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("User not logged in");
            var sale = await _saleRepo.GetByIdAsync(saleId);
            if (sale == null)
                throw new NotFoundException($"Sale with id {saleId}  not found");
            if (sale.UserId.ToString() != userId)
                throw new BadRequestException($"User Id invalid");

            await _saleRepo.DeleteAsync(sale);
            return true;
        }

        public async Task<SalesResponseDto> EditSale(SalesResponseDto model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("User not logged in");

            UserProfile user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with id: {userId} not found");

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
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("User not logged in");

            var sale = await _saleRepo.GetByIdAsync(SaleId);
            if (sale == null)
                throw new NotFoundException("Sale id not found");

            return _mapper.Map<SalesResponseDto>(sale);
        }

        public IEnumerable<SalesResponseDto> GetUserSales()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("User not logged in");
            // = await  _saleRepo.GetByAsync(s => s.UserId == UserId);
            var sales = _saleRepo.GetQueryable(p => p.UserId.ToString() == userId).OrderBy(i => i.Id);
            if (sales == null)
                throw new NotFoundException("User id was incorrect");
            return _mapper.Map<IEnumerable<SalesResponseDto>>(sales);
        }


    }
}
