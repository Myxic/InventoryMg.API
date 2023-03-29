using AutoMapper;
using InventoryMg.BLL.DTOs;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            /*await _roleManager.CreateAsync(new AppRole { Name = "Customer", Id = Guid.NewGuid().ToString() });
            var getRole = _roleManager.Roles.Where(r => r.Name == "Customer").FirstOrDefault();*/
            await _userManager.AddToRoleAsync(user, "Customer");

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


        public async Task<AuthResult> GetNewJwtRefreshToken(TokenRequest tokenRequest)
        {
            var result = await VerifyAndGenerateToken(tokenRequest);
            if (result.Result == false)
                throw new Exceptions.NotImplementedException("Invalid Tokens");

            return result;
        }
        private async Task<AuthResult> GenerateJwtToken(UserProfile user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var claims = await GetAllValidClaims(user);

            //Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
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

        private async Task<IList<Claim>> GetAllValidClaims(UserProfile user)
        {
            var _options = new IdentityOptions();
            var claims = new List<Claim>()
            {
                 new Claim("Id", user.Id),
                  new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };
            //getting claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //get the user role and add to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            //convert roles to claims
            foreach (var userRole in userRoles)
            {
               
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }

                
            }
            return claims;

        }
        private string RandomStringGenrator(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;// for dev
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                    var utcExpiryDate = long.Parse(tokenInVerification.Claims
                        .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                    var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                    if (expiryDate > DateTime.Now)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Expired Token"
                            }
                        };
                    }

                    var storedToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                    if (storedToken == null)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Invalid Token"
                            }
                        };
                    }

                    if (storedToken.IsUsed)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Invalid Token"
                            }
                        };
                    }

                    if (storedToken.IsRevoked)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Invalid Token"
                            }
                        };
                    }

                    var jti = tokenInVerification.Claims
                        .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (storedToken.JwtId != jti)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Invalid Token"
                            }
                        };
                    }

                    if (storedToken.ExpiryDate < DateTime.UtcNow)
                    {
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Expired Token"
                            }
                        };
                    }

                    storedToken.IsUsed = true;
                    _dbContext.RefreshTokens.Update(storedToken);
                    await _dbContext.SaveChangesAsync();

                    var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                    return await GenerateJwtToken(dbUser);


                }
            }
            catch (Exception ex)
            {
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                            {
                                $"{ex.Message}",
                                $"{ex.StackTrace}"
                            }
                };
            }
            return new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                            {
                                $"server error",

                            }
            };
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }

    }
}
