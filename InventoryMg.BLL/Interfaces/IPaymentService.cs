using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.DAL.Entities;
using PayStack.Net;

namespace InventoryMg.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<TransactionInitializeResponse> InitalizePayment(PaymentRequest request);
        Task<TransactionVerifyResponse> VerifyPayment(string reference);
        Task<IEnumerable<Transaction>> GetPayments();
        Task<Transaction> GetPaymentByid(string id);
    }
}
