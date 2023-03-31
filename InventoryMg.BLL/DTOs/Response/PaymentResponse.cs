using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.DTOs.Response
{
    public class PaymentResponse
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string TrxnRef { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}
