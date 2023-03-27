using InventoryMg.DAL.Entities;
using InventoryMg.BLL.DTOs;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryMg.BLL.Exceptions;

namespace InventoryMg.BLL.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserProfile> _userManager;
        // private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<AppRole> _roleManager;

        public AuthenticationService(UserManager<UserProfile> userManager, IConfiguration configuration, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            //   _jwtConfig = jwtConfig;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<AuthResult> CreateUser(UserRegistration request)
        {

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exceptions.NotImplementedException($"User already exists with Email {request.Email}");

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
            };
            //generate token
           await _roleManager.CreateAsync(new AppRole { Name = "Customer", Id= Guid.NewGuid().ToString()});
            var getRole = _roleManager.Roles.Where(r => r.Name == "Customer").FirstOrDefault();
          await  _userManager.AddToRoleAsync(user,getRole.Name);

            var token = GenerateJwtToken(user);


            return new AuthResult()
            {
                Result = true,
                Token = token,
                Errors = null
            };
        }

        public async Task<AuthenticationResponse> UserLogin(LoginRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
                throw new NotFoundException($"Invalid email/password");

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser,request.Password);
            if (!isCorrect)
            {
                throw new NotFoundException($"Invalid email/password");
            }

            var jwtToken = GenerateJwtToken(existingUser);
            return new AuthenticationResponse()
            {
                JwtToken = jwtToken,
                FullName = existingUser.FullName
            };
            
        }

        private string GenerateJwtToken(UserProfile user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            //Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", user.Id),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = JwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

    }
}
