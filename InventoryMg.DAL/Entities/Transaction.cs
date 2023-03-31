using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.DAL.Entities
{
    public class Transaction : BaseEntity
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string TrxnRef { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }

        [ForeignKey("UserProfile")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }
    }
}
