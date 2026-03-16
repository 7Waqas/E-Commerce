using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property — one category has many products
        public List<Product> Products { get; set; } = new();
    }
}