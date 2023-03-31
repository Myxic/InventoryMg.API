using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStack.Net;

namespace InventoryMg.BLL.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IRepository<Transaction> _transRepo;
        private readonly string token;
        private readonly UserManager<UserProfile> _userManager;

        private PayStackApi PayStack { get; set; }

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, UserManager<UserProfile> userManager)
        {
            _unitOfWork = unitOfWork;
            _transRepo = _unitOfWork.GetRepository<Transaction>();
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            token = _configuration["Payment:PaystackTestKey"];
            PayStack = new PayStackApi(token);
        }
        public async Task<TransactionInitializeResponse> InitalizePayment(PaymentRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException($"User with id {request.UserId} not found");

            TransactionInitializeRequest createRequest = new()
            {
                AmountInKobo = request.Amount * 100,
                Email = request.Email,
                Currency = "NGN",
                Reference = Generate().ToString(),
                CallbackUrl = "https://localhost:7242/api/Payment/verify-payment"

            };
            TransactionInitializeResponse response = PayStack.Transactions.Initialize(createRequest);
            if (response.Status)
            {
                var transaction = new Transaction()
                {
                    Name = request.Name,
                    Amount = request.Amount,
                    TrxnRef = createRequest.Reference,
                    Email = request.Email,
                    Status = false,
                    UserId = new Guid(request.UserId)

                };
                await _transRepo.AddAsync(transaction);
                return response;

            }
            throw new Exceptions.NotImplementedException("the payment was unable to go through");
        }

        public async Task<TransactionVerifyResponse> VerifyPayment(string reference)
        {
            TransactionVerifyResponse response = PayStack.Transactions.Verify(reference);
            if (response.Data.Status == "success")
            {
                var transaction = await _transRepo.GetSingleByAsync(x => x.TrxnRef == reference);

                if (transaction != null)
                {
                    if (!transaction.Status)
                    {
                        transaction.Status = true;

                        await _transRepo.UpdateAsync(transaction);

                        return response;
                    }

                    throw new Exceptions.NotImplementedException("You have already verified this payment");
                }
            }
            throw new Exceptions.NotImplementedException("Was not able to complete this request");
        }

        public async Task<IEnumerable<Transaction>> GetPayments()
        {
            var payments = await _transRepo.GetAllAsync();
            if (payments != null)
            {

                return payments;
            }
            throw new NotFoundException("No transactions");
        }

        public async Task<Transaction> GetPaymentByid(string id)
        {

            var result = await _transRepo.GetByIdAsync(new Guid(id));
            if (result != null)
            {
                return result;
            }
            throw new NotFoundException("Invalid userid");
        }

        private static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            return rand.Next(100000000, 999999999);
        }
    }
}
