using AutoMapper;
using BusinessObject.BusinessObject.Appointment.Request;
using BusinessObject.BusinessObject.Appointment.Respond;
using Repositories.CustomRepositories.Implements;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAppointmentAsync(AddAppointmentRequest dto)
        {
            // 1. Business rule: vehicle cannot be double-booked at same time
            //bool conflict = await _appointmentRepository.CheckVehicleConflictAsync(dto.VehicleId, dto.AppointmentDate);
            var appointmentRepository = _unitOfWork.GetCustomRepository<IAppointmentRepository>();
            bool conflict = await appointmentRepository.IsVehicleBookedAsync(dto.VehicleId, dto.AppointmentDate, dto.AppointmentDate.AddHours(1));
            if (conflict)
                return false;

            // 2. Find or create customer
            var customer = await appointmentRepository.GetCustomerByEmailAsync(dto.CustomerEmail);
            if (customer == null)
            {
                customer = new Customer
                {
                    Name = dto.CustomerName,
                    Phone = dto.CustomerPhone,
                    Email = dto.CustomerEmail,
                    Address = dto.CustomerAddress
                };
                customer = await appointmentRepository.AddCustomerAsync(customer);
            }

            // 3. Create appointment
            var appointment = new Appointment
            {
                CustomerId = customer.CustomerId,
                VehicleId = dto.VehicleId,
                AppointmentDate = dto.AppointmentDate
            };

            //await appointmentRepository.AddAppointmentAsync(appointment);
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            await appointmentRepo.AddAsync(appointment);
            await _unitOfWork.SaveAsync(); 
            return true;
        }

        public async Task<IEnumerable<GetAppointmentRespond>> GetAppointmentsAsync()
        {
            var appointmentRepository = _unitOfWork.GetCustomRepository<IAppointmentRepository>();
            var appointments = await appointmentRepository.GetAppointmentsAsync();

            return appointments.Select(a => new GetAppointmentRespond
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                CustomerName = a.Customer.Name,
                CustomerPhone = a.Customer.Phone,
                CustomerEmail = a.Customer.Email,
                VehicleId = a.Vehicle.VehicleId,
                VehicleModel = a.Vehicle.Model,
                VehicleColor = a.Vehicle.Color,
                VehicleVersion = a.Vehicle.Version ?? ""
            });
        }

        public async Task<GetAppointmentRespond?> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointmentRepository = _unitOfWork.GetCustomRepository<IAppointmentRepository>();
            var a = await appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (a == null) return null;

            return new GetAppointmentRespond
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                CustomerName = a.Customer.Name,
                CustomerPhone = a.Customer.Phone,
                CustomerEmail = a.Customer.Email,
                VehicleId = a.Vehicle.VehicleId,
                VehicleModel = a.Vehicle.Model,
                VehicleColor = a.Vehicle.Color,
                VehicleVersion = a.Vehicle.Version ?? ""
            };
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null) return false;
            appointmentRepo.DeleteAsync(appointment);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
