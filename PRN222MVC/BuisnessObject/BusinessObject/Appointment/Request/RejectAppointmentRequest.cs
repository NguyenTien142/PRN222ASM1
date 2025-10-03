using System.ComponentModel.DataAnnotations;

namespace BusinessObject.BusinessObject.Appointment.Request
{
    public class RejectAppointmentRequest
    {
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
