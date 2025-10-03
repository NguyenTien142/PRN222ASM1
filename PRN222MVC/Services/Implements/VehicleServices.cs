using AutoMapper;
using Azure.Core;
using BusinessObject.BusinessObject.VehicleModels.Request;
using BusinessObject.BusinessObject.VehicleModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

namespace Services.Implements
{
    public class VehicleServices : IVehicleServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int ADMIN_INVENTORY_ID = 4; // InventoryId cố định cho admin

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

        public async Task<List<GetAdminVehicleResponse>> GetVehiclesByInventoryIdAsync(int inventoryId)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            var vehicleInventories = await vehicleRepo.GetVehiclesByInventoryIdAsync(ADMIN_INVENTORY_ID);

            // Sử dụng AutoMapper mapping đã setup trong MapperProfile
            var result = _mapper.Map<List<GetAdminVehicleResponse>>(vehicleInventories);

            return result;
        }

        public async Task UpdateVehicleInventoryAsync(UpdateVehicleInventoryRequest request)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            await vehicleRepo.UpdateVehicleInventoryAsync(request);
        }

        public async Task DeleteVehicleInventoryAsync(int id)
        {
            var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();
            await vehicleRepo.DeleteVehicleInventoryAsync(id);
        }

        // Enhanced ImportVehiclesAsync with better resource management
        public async Task<ImportVehicleResponse> ImportVehiclesAsync(IFormFile file)
        {
            var response = new ImportVehicleResponse();
            const int ADMIN_INVENTORY_ID = 2;

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Errors.Add("File is empty or not provided");
                    return response;
                }

                // Validate file extension
                var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    response.Errors.Add("Invalid file format. Only CSV and Excel files are supported.");
                    return response;
                }

                var vehicles = new List<ImportVehicleRequest>();

                // Parse file based on extension (without cancellation support for now)
                if (fileExtension == ".csv")
                {
                    vehicles = await ParseCsvFile(file);
                }
                else
                {
                    vehicles = await ParseExcelFile(file);
                }

                if (!vehicles.Any())
                {
                    response.Errors.Add("No valid data found in the file");
                    return response;
                }

                // Process each vehicle
                var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();

                foreach (var vehicleData in vehicles)
                {
                    try
                    {
                        // Validate data
                        var validationErrors = ValidateVehicleData(vehicleData);
                        if (validationErrors.Any())
                        {
                            response.ErrorCount++;
                            response.Errors.AddRange(validationErrors.Select(e => $"Row {vehicles.IndexOf(vehicleData) + 2}: {e}"));
                            continue;
                        }

                        // Check if vehicle already exists
                        if (await vehicleRepo.VehicleExistsAsync(vehicleData.Model, vehicleData.Color, vehicleData.Version ?? ""))
                        {
                            response.Warnings.Add($"Vehicle {vehicleData.Model} - {vehicleData.Color} already exists, skipping");
                            continue;
                        }

                        // Create vehicle
                        var createVehicleRequest = new CreateVehicleRequest
                        {
                            CategoryId = vehicleData.CategoryId,
                            Color = vehicleData.Color,
                            Price = vehicleData.Price,
                            ManufactureDate = vehicleData.ManufactureDate,
                            Model = vehicleData.Model,
                            Version = vehicleData.Version,
                            Image = vehicleData.Image
                        };

                        var vehicleId = await vehicleRepo.CreateVehicleAsync(createVehicleRequest);

                        // Create vehicle inventory
                        var createInventoryRequest = new CreateVehicleInventoryRequest
                        {
                            VehicleId = vehicleId,
                            InventoryId = ADMIN_INVENTORY_ID,
                            Quantity = vehicleData.Quantity
                        };

                        await vehicleRepo.CreateVehicleInventoryAsync(createInventoryRequest);

                        response.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCount++;
                        response.Errors.Add($"Row {vehicles.IndexOf(vehicleData) + 2}: {ex.Message}");
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Import failed: {ex.Message}");
                return response;
            }
        }

        // FIXED: Remove CancellationToken parameter to match updated signature
        private async Task<List<ImportVehicleRequest>> ParseCsvFile(IFormFile file)
        {
            var vehicles = new List<ImportVehicleRequest>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            string line;
            bool isHeader = true;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue; // Skip header row
                }

                var values = line.Split(',');
                if (values.Length >= 8)
                {
                    var vehicle = ParseVehicleData(values);
                    if (vehicle != null)
                    {
                        vehicles.Add(vehicle);
                    }
                }
            }

            return vehicles;
        }

        // FIXED: Remove CancellationToken parameter to match updated signature
        private async Task<List<ImportVehicleRequest>> ParseExcelFile(IFormFile file)
        {
            var vehicles = new List<ImportVehicleRequest>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(file.OpenReadStream());
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip header)
            {
                var values = new string[8];
                for (int col = 1; col <= Math.Min(8, worksheet.Dimension.Columns); col++)
                {
                    values[col - 1] = worksheet.Cells[row, col].Value?.ToString() ?? "";
                }

                var vehicle = ParseVehicleData(values);
                if (vehicle != null)
                {
                    vehicles.Add(vehicle);
                }
            }

            return vehicles;
        }

        private ImportVehicleRequest? ParseVehicleData(string[] values)
        {
            try
            {
                // Template: Category, Color, Price, ManufactureDate, Model, Version, Image, Quantity
                return new ImportVehicleRequest
                {
                    CategoryId = int.Parse(values[0]),
                    Color = values[1].Trim(),
                    Price = decimal.Parse(values[2]),
                    ManufactureDate = DateOnly.ParseExact(values[3], "M/d/yyyy", CultureInfo.InvariantCulture),
                    Model = values[4].Trim(),
                    Version = string.IsNullOrWhiteSpace(values[5]) ? null : values[5].Trim(),
                    Image = string.IsNullOrWhiteSpace(values[6]) ? null : values[6].Trim(),
                    Quantity = int.Parse(values[7])
                };
            }
            catch
            {
                return null; // Invalid data, skip this row
            }
        }

        private List<string> ValidateVehicleData(ImportVehicleRequest vehicle)
        {
            var errors = new List<string>();

            if (vehicle.CategoryId <= 0)
                errors.Add("Invalid Category ID");

            if (string.IsNullOrWhiteSpace(vehicle.Color))
                errors.Add("Color is required");

            if (vehicle.Price <= 0)
                errors.Add("Price must be greater than 0");

            if (string.IsNullOrWhiteSpace(vehicle.Model))
                errors.Add("Model is required");

            if (vehicle.Quantity < 0)
                errors.Add("Quantity cannot be negative");

            if (!string.IsNullOrWhiteSpace(vehicle.Image) && !Uri.IsWellFormedUriString(vehicle.Image, UriKind.Absolute))
                errors.Add("Invalid image URL");

            return errors;
        }

        // Add this method to VehicleServices class

        public async Task<AddVehicleWithInventoryResponse> AddVehicleWithInventoryAsync(AddVehicleWithInventoryRequest request)
        {
            var response = new AddVehicleWithInventoryResponse();
            const int ADMIN_INVENTORY_ID = 2;

            try
            {
                var vehicleRepo = _unitOfWork.GetCustomRepository<IVehicleRepository>();

                // Check if vehicle already exists
                if (await vehicleRepo.VehicleExistsAsync(request.Model, request.Color, request.Version ?? ""))
                {
                    response.Success = false;
                    response.Message = $"Vehicle {request.Model} - {request.Color} already exists";
                    return response;
                }

                // Create vehicle
                var createVehicleRequest = new CreateVehicleRequest
                {
                    CategoryId = request.CategoryId,
                    Color = request.Color,
                    Price = request.Price,
                    ManufactureDate = request.ManufactureDate,
                    Model = request.Model,
                    Version = request.Version,
                    Image = request.Image
                };

                var vehicleId = await vehicleRepo.CreateVehicleAsync(createVehicleRequest);

                // Create vehicle inventory
                var createInventoryRequest = new CreateVehicleInventoryRequest
                {
                    VehicleId = vehicleId,
                    InventoryId = ADMIN_INVENTORY_ID,
                    Quantity = request.Quantity
                };

                await vehicleRepo.CreateVehicleInventoryAsync(createInventoryRequest);

                response.Success = true;
                response.Message = "Vehicle added successfully";
                response.VehicleId = vehicleId;

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to add vehicle";
                response.Error = ex.Message;
                return response;
            }
        }
    }
}
