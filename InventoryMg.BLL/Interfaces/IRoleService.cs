using InventoryMg.BLL.DTOs.Response;
using InventoryMg.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<AppRole>> GetAllRoles();

        Task<RoleResult> CreateRole(string name);

        Task<IEnumerable<UserProfile>> GetAllUser();

        Task<RoleResult> AddUserToRole(string email, string roleName);
        Task<IList<string>> GetUserRoles(string email);

        Task<RoleResult> RemoveUserFromRole(string email, string roleName);
    }
}
