using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.VehicleModels.Request
{
    public class ImportVehicleRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Color { get; set; } = null!;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        public DateOnly ManufactureDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = null!;

        [StringLength(50)]
        public string? Version { get; set; }

        [Url]
        public string? Image { get; set; }

        [Required]
        [Range(0, 9999)]
        public int Quantity { get; set; }
    }
}
