using System.ComponentModel.DataAnnotations;

namespace BusinessObject.BusinessObject.Appointment.Request
{
    public class ApproveAppointmentRequest
    {
        [Required]
        public int AppointmentId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
