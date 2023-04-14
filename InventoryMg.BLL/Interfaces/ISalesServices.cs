using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;

namespace InventoryMg.BLL.Interfaces
{
    public interface ISalesServices
    {

        IEnumerable<SalesResponseDto> GetUserSales();

        Task<SalesResponseDto> AddSale(SalesRequestDto model);

        Task<SalesResponseDto> GetSaleById(Guid SaleId);

        Task<SalesResponseDto> EditSale(SalesResponseDto model);

        Task<bool> DeleteSale(Guid saleId);
    }
}
