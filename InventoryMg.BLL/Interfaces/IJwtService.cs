using InventoryManager.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserProfile userProfile);
    }
}
