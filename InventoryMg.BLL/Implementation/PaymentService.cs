using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStack.Net;
using System.Security.Claims;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        private PayStackApi PayStack { get; set; }

        public PaymentService(IUnitOfWork unitOfWork,
            IConfiguration configuration, IMapper mapper,
            UserManager<UserProfile> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _transRepo = _unitOfWork.GetRepository<Transaction>();
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            token = _configuration["Payment:PaystackTestKey"];
            PayStack = new PayStackApi(token);
        }
        public async Task<TransactionInitializeResponse> InitalizePayment(PaymentRequest request)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("Invalid User id please authenticate");
            UserProfile user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with id {userId} not found");

            TransactionInitializeRequest createRequest = new()
            {
                AmountInKobo = request.Amount * 100,
                Email = user.Email,
                Currency = "NGN",
                Reference = Generate().ToString(),
                CallbackUrl = "https://localhost:7242/api/Payment/verify-payment"

            };
            TransactionInitializeResponse response = PayStack.Transactions.Initialize(createRequest);
            if (response.Status)
            {
                var transaction = new Transaction()
                {
                    Name = user.FullName,
                    Amount = request.Amount,
                    TrxnRef = createRequest.Reference,
                    Email = user.Email,
                    Status = false,
                    UserId = new Guid(userId)

                };
                await _transRepo.AddAsync(transaction);
                return response;

            }
            throw new Exceptions.NotImplementedException("the payment was unable to go through");
        }

        public async Task<TransactionVerifyResponse> VerifyPayment(string reference)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("Invalid User id please authenticate");
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
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("Invalid User id please authenticate");

            var payments = await _transRepo.GetAllAsync();
            if (payments != null)
            {

                return payments;
            }
            throw new NotFoundException("No transactions");
        }

        public async Task<Transaction> GetPaymentByid(string id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new NotFoundException("Invalid User id please authenticate");
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
