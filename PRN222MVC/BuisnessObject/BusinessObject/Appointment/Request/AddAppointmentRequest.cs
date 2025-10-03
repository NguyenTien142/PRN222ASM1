using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.Appointment.Request
{
    public class AddAppointmentRequest
    {
        //[Required]
        //public int CustomerId { get; set; }
        [Required]
        public int VehicleId { get; set; }
        [Required]
        public string CustomerName { get; set; } 
        [Required]
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
