using InventoryMg.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryMg.BLL.DTOs.Response
{
    public class SalesResponseDto
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public long Quantity { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public Guid ProductId { get; set; }
    }
}
