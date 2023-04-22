using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all-app-roles")]
        [SwaggerOperation(Summary = "Get all available roles on the application", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return the all the available roles")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound,"Not Found")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }

        [HttpPost("create-a-role")]
        [SwaggerOperation(Summary = "Create a role", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status201Created, "Return the just created role name")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> CreateRole(string name)
        {
            RoleResult obj = await _roleService.CreateRole(name);
            if (obj.result == false)
            {
                return BadRequest(obj);
            }



            return StatusCode(201,obj);
        }

        [HttpGet("get-all-users")]
        [SwaggerOperation(Summary = "Get all avalible users on the application", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a list of users")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _roleService.GetAllUser();
            return Ok(result);
        }

        [HttpPost("add-user-to-role")]
        [SwaggerOperation(Summary = "add a user to a role", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a success message")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _roleService.AddUserToRole(email, roleName);
            if (result.result)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpGet("get-user-roles")]
        [SwaggerOperation(Summary = "get all the roles belonging to a user", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a list of role id belonging to a user")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var result = await _roleService.GetUserRoles(email);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost("remove-user-from-role")]
        [SwaggerOperation(Summary = "remove a user from a role", Description = "Requires admin authorization")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return a success message")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var result = await _roleService.RemoveUserFromRole(email, roleName);
            if (result.result)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


    }
}
