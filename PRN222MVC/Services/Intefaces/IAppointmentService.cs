using BusinessObject.BusinessObject.Appointment.Request;
using BusinessObject.BusinessObject.Appointment.Respond;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Intefaces
{
    public interface IAppointmentService
    {
        Task<bool> AddAppointmentAsync(AddAppointmentRequest dto);
        Task<IEnumerable<GetAppointmentRespond>> GetAppointmentsAsync();
        Task<GetAppointmentRespond?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> CancelAppointmentAsync(int appointmentId);
    }
}
