using InventoryManager.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.DAL.Entities
{
    public class Sale : BaseEntity
    {
        public string Name { get; set; }
        public Category Category { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("UserProfile")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public IList<Product> Products { get; set; }
    }
}
