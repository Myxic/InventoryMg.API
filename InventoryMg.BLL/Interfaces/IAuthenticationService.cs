using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> CreateUser(UserRegistration request);
        Task<AuthenticationResponse> UserLogin(LoginRequest request);
    }
}
