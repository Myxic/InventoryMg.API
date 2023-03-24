using InventoryMg.BLL.DTOs.Request;
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

        [HttpGet]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration userRegistrationRequestDto)
        {
            if (ModelState.IsValid)
            {

            }

            return BadRequest();
        }

       
    }
}
