using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.VehicleModels.Request
{
    public class CreateVehicleInventoryRequest
    {   public int VehicleId { get; set; }
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
    }
}
