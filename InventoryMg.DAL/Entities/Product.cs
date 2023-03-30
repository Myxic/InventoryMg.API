using InventoryMg.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMg.DAL.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Category Category { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }

        public string? ProductImagePath { get; set; }

        [ForeignKey("UserProfile")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }

        public IList<Sale> Sales { get; set; } = new List<Sale>();
    }
}
