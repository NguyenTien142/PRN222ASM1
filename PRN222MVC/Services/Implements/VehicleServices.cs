using AutoMapper;
using BusinessObject.BusinessObject.VehicleModels.Request;
using BusinessObject.BusinessObject.VehicleModels.Respond;
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
    public class VehicleServices : IVehicleServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<GetDetailVehicleRespond?> AddVehicle(GetDetailVehicleRespond vehicle)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteVehicle(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetVehicleRespond>> GetAllVehicle()
        {
            var vehiclesRepo = _unitOfWork.GetRepository<Vehicle>();
            var vehicles = await vehiclesRepo.GetAllAsync();
            var vehicleResponds = _mapper.Map<List<GetVehicleRespond>>(vehicles.ToList());
            return vehicleResponds;
        }

        public async Task<GetDetailVehicleRespond?> GetVehicleById(int vehicleId)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            var vehicle = await vehicleRepo.GetDetailVehiclesAsync(vehicleId);
            var vehicleRespond = _mapper.Map<GetDetailVehicleRespond>(vehicle);

            return vehicleRespond;
        }

        public Task<bool> UpdateVehicle(UpdateVehicleRequest vehicle)
        {
            throw new NotImplementedException();
        }
    }
}
