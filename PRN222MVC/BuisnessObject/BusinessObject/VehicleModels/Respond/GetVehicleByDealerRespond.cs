using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.VehicleModels.Respond
{
    public class GetVehicleByDealerRespond
    {
        public int VehicleId { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public DateTime ManufactureDate { get; set; }
        public string Image { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public int DealerId { get; set; }
        public string Address { get; set; }
        public string DealerType { get; set; }
    }
}
