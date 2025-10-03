using System;

namespace BusinessObject.BusinessObject.Appointment.Respond
{
    public class GetPendingAppointmentResponse
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Customer info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        // Vehicle info
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleColor { get; set; } = string.Empty;
        public string VehicleVersion { get; set; } = string.Empty;

        // Dealer info (nếu cần)
        public string DealerName { get; set; } = string.Empty;
        public string DealerType { get; set; } = string.Empty;
    }
}
