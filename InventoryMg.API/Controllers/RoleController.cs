using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Route("get-all-app-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }

        [HttpPost]
        [Route("create-a-role")]
        public async Task<IActionResult> CreateRole(string name)
        {
            RoleResult obj = await _roleService.CreateRole(name);
            if (obj.result == false)
            {
                return BadRequest(obj);
            }



            return StatusCode(201,obj);
        }

        [HttpGet]
        [Route("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _roleService.GetAllUser();
            return Ok(result);
        }

        [HttpPost]
        [Route("add-user-to-role")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _roleService.AddUserToRole(email, roleName);
            if (result.result)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpGet]
        [Route("get-user-roles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var result = await _roleService.GetUserRoles(email);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("remove-user-from-role")]
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
