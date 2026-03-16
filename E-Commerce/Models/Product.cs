using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product URL is required")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Buy Now URL")]
        public string ProductUrl { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Category relationship ──────────────────────────────
        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // Not stored in DB — only for upload form
        [NotMapped]
        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }
    }
}