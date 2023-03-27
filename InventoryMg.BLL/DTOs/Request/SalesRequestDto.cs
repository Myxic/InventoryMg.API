using InventoryMg.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryMg.BLL.DTOs.Request
{
    public class SalesRequestDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public long Quantity { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public string ProductId { get; set; }
    }
}
