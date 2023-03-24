using InventoryManager.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.DAL.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Category Category { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }

        [ForeignKey("UserProfile")]
        public int UserId { get; set; }
        public UserProfile User { get; set; }

        public IList<Sale> Sales { get; set; } = new List<Sale>();
    }
}
