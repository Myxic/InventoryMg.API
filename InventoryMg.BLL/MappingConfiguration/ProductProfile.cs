using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.DAL.Entities;

namespace InventoryMg.BLL.MappingConfiguration
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductViewRequest, Product>();
            CreateMap<Product,ProductView>();
            CreateMap<ProductView, Product>();
        }
    }
}
