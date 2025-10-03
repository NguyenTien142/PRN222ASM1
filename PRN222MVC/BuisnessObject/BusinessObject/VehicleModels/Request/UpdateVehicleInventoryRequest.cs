using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.VehicleModels.Request
{
    public class UpdateVehicleInventoryRequest
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Version { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
    }
}
