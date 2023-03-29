using InventoryMg.BLL.DTOs.Response;
using InventoryMg.BLL.Exceptions;
using InventoryMg.BLL.Interfaces;
using InventoryMg.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace InventoryMg.BLL.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<UserProfile> _userManger;
        private readonly RoleManager<AppRole> _roleManager;
        public RoleService(
            UserManager<UserProfile> userManger,
            RoleManager<AppRole> roleManager
            )
        {
            _userManger = userManger;
            _roleManager = roleManager;

        }

        public async Task<RoleResult> AddUserToRole(string email, string roleName)
        {
            //check if user exist
            var user =  await _userManger.FindByEmailAsync(email);
            if(user == null )
            {
                throw new NotFoundException($"User with email: {email} was not found");
            }
            //check if role exist
            var roleExits = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExits)
            {
                throw new NotFoundException($"Role name:{roleName} does not exist");
            }

            var result = await _userManger.AddToRoleAsync(user, roleName);
            if(result.Succeeded)
            {
                return new RoleResult()
                {
                    result = true,
                    message = $"User: {user.FullName} was successfully added to the role {roleName}"
                };
            }
            throw new Exceptions.NotImplementedException("Unable to add user to a role");
                //check if user was added to the role successfully

            }

        public async Task<RoleResult> CreateRole(string name)
        {
            //check if role already exits
            var roleExits = await _roleManager.RoleExistsAsync(name);

            if (!roleExits)
            {

                var roleResult = await _roleManager.CreateAsync(new AppRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                });
                //check if role was added successfully
                if (roleResult.Succeeded)
                {
                    return new RoleResult() { message = $"Role {name} has been added successfully", result = true };
                }

                return new RoleResult() { message = $"Role {name} has not been added", result = false };
            }

            return new RoleResult() { message = "Role already exist", result = false };
        }

        public async Task<IEnumerable<AppRole>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<IEnumerable<UserProfile>> GetAllUser()
        {
            var users = await _userManger.Users.ToListAsync();
            return users;

        }
    }
}
