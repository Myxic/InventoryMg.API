using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.DTOs.Request
{
    public class PaymentRequest
    {
        public string UserId { get; set; }  
        public string Name { get; set; }
        public string Email { get; set; }
        public int Amount { get; set; }
    }
}
