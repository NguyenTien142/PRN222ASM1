using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.Appointment.Respond
{
    public class GetAppointmentRespond
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }

        // Customer info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        // Vehicle info
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleColor { get; set; } = string.Empty;
        public string VehicleVersion { get; set; } = string.Empty;
    }
}
