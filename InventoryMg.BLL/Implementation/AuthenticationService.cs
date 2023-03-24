using InventoryManager.DAL.Entities;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Configurations;
using Microsoft.AspNetCore.Identity;

namespace InventoryMg.BLL.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationService(UserManager<IdentityUser> userManager, JwtConfig jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig;
        }

        public async Task<string> CreateUser(UserRegistration request)
        {

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"User already exists with Email {request.Email}");

            UserProfile user = new()
            {
                FullName = request.FirstName + " " + request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,

            };
            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}");
            }

            return result.ToString();
        }

        public Task<AuthenticationResponse> UserLogin(LoginRequest request)
        {
            throw new NotImplementedException();
        }


    }
}
