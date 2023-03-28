using InventoryMg.BLL.DTOs;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration userRegistrationRequestDto)
        {
            if (ModelState.IsValid)
            {
                AuthResult response = await _authenticationService.CreateUser(userRegistrationRequestDto);

                if (response.Result)
                {
                    //  return CreatedAtAction("Register",response);
                    return Ok(response);
                }

                return BadRequest(response);
            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                    {
                    "Invalid payload"
                    }
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {

                AuthenticationResponse response = await _authenticationService.UserLogin(loginRequest);

                if (response.FullName != null)
                {
                    //  return CreatedAtAction("Register",response);2
                    return Ok(response);
                }

                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                    "Invalid Email/Password"
                    }
                });

            }
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                    {
                    "Invalid payload"
                    }
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.GetNewJwtRefreshToken(tokenRequest);
                if (result.Result == false)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                {
                    "Invalid Tokens"
                },
                        Result = false
                    });
                }

                return Ok(result);
            }
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Invalid parameters"
                },
                Result = false
            });
        }

    }
}
