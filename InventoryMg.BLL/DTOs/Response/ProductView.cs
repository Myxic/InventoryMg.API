using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryMg.BLL.DTOs.Response
{
    public class ProductView
    {
        public int? Id { get; set; }
        [Required, StringLength(50, ErrorMessage = "Product name should be between 5 to 50 characters", MinimumLength = 5)]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please Select a valid category")]
        public Category Category { get; set; }
        [Required]
        public long Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required, StringLength(50, ErrorMessage = "Product brandname should be between 5 to 50 characters", MinimumLength = 2)]
        public string BrandName { get; set; }
        public Guid UserId { get; set; }
       
    }
}
