using AutoMapper;
using InventoryMg.BLL.DTOs;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryMg.BLL.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserProfile> _userManager;
        // private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationService(UserManager<UserProfile> userManager,
            IConfiguration configuration, RoleManager<AppRole> roleManager,
            IMapper mapper, ApplicationDbContext dbContext, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            //   _jwtConfig = jwtConfig;
            _configuration = configuration;
            _roleManager = roleManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthResult> CreateUser(UserRegistration request)
        {

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exceptions.NotImplementedException($"User already exists with Email {request.Email}");
            UserProfile user = _mapper.Map<UserProfile>(request);

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}");
            };
            //generate token
            await _roleManager.CreateAsync(new AppRole { Name = "Customer", Id = Guid.NewGuid().ToString() });
            var getRole = _roleManager.Roles.Where(r => r.Name == "Customer").FirstOrDefault();
            await _userManager.AddToRoleAsync(user, getRole.Name);

            var token = await GenerateJwtToken(user);


            return token;
        }

        public async Task<AuthenticationResponse> UserLogin(LoginRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
                throw new NotFoundException($"Invalid email/password");

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (!isCorrect)
            {
                throw new NotFoundException($"Invalid email/password");
            }

            var jwtToken = await GenerateJwtToken(existingUser);
            return new AuthenticationResponse()
            {
                JwtToken = jwtToken.Token,
                RefreshToken = jwtToken.RefreshToken,
                FullName = existingUser.FullName
            };

        }

        private async Task<AuthResult> GenerateJwtToken(UserProfile user)
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
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = JwtTokenHandler.WriteToken(token);


            //refresh token

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomStringGenrator(23),//generate a refresh token
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id,

            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();


            return new AuthResult()
            {
                Result = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Errors = null
            };
        }

        private string RandomStringGenrator(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

        }

    }
}
