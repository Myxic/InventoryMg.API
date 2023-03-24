using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.DTOs.Response
{
    public class AuthenticationResponse
    {
        public string JwtToken { get; set; }
        public string FullName { get; set; }

    }
}
