using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.VehicleModels.Request
{
    public class CreateVehicleRequest
    {
        public int CategoryId { get; set; }

        public string Color { get; set; } = null!;

        public decimal Price { get; set; }

        public DateOnly ManufactureDate { get; set; }

        public string Model { get; set; } = null!;

        public string? Version { get; set; }

        public string? Image { get; set; }
    }
}
