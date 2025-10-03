using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.BusinessObject.VehicleModels.Request
{
    public class AddVehicleWithInventoryRequest
    {
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Color is required")]
        [StringLength(50, ErrorMessage = "Color must be between 1 and 50 characters")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Manufacture date is required")]
        public DateOnly ManufactureDate { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model must be between 1 and 100 characters")]
        public string Model { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Version must be less than 50 characters")]
        public string? Version { get; set; }

        [Url(ErrorMessage = "Image must be a valid URL")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, 9999, ErrorMessage = "Quantity must be between 0 and 9999")]
        public int Quantity { get; set; }
    }
}