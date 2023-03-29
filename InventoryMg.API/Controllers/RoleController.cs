using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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



            return Ok(obj);
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
    }
}
