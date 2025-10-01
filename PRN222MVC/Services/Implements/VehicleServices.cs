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

        public async Task<bool> AddVehicle(CreateVehicleRequest request)
        {
            var vehicleRepo = _unitOfWork.GetRepository<Vehicle>();
            var vehicle = _mapper.Map<Vehicle>(request);
            await vehicleRepo.AddAsync(vehicle);
            await _unitOfWork.SaveAsync();
            return true;
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

        public async Task<List<GetVehicleByDealerRespond>> GetVehicleByDealerId(int dealerId)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            var vehicles = await vehicleRepo.GetVehicleBuyDealerIdAsync(dealerId);
            var vehicleResponds = _mapper.Map<List<GetVehicleByDealerRespond>>(vehicles.ToList());
            return vehicleResponds;
        }

        public async Task<GetDetailVehicleRespond?> GetVehicleById(int vehicleId)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            var vehicle = await vehicleRepo.GetDetailVehiclesAsync(vehicleId);
            var vehicleRespond = _mapper.Map<GetDetailVehicleRespond>(vehicle);

            return vehicleRespond;
        }

        public async Task<bool> UpdateVehicle(int vehicleId, UpdateVehicleRequest request)
        {
            var vehicleRepo = _unitOfWork.GetRepository<Vehicle>();
            var vehicle = await vehicleRepo.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            if(request.CategoryId > 0)
                vehicle.CategoryId = request.CategoryId;
            if (request.Color != null)
                vehicle.Color = request.Color;
            if (request.Price >= 0)
                vehicle.Price = request.Price;
            vehicle.ManufactureDate = request.ManufactureDate;
            if (request.Model != null)
                vehicle.Model = request.Model;
            if (vehicle.Version != null)
                vehicle.Version = request.Version;
            if (vehicle.Image != null)
                vehicle.Image = request.Image;

            vehicleRepo.Update(vehicle);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
