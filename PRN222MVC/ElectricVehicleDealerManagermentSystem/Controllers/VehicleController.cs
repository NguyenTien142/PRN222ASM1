using BusinessObject.BusinessObject.VehicleModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject.BusinessObject.VehicleModels.Respond;
using Microsoft.AspNetCore.Http;
using Services.Implements;
using Services.Intefaces;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Route("vehicle")]
    public class VehicleController : Controller
    {
        private readonly IVehicleServices _vehicleServices;
        private readonly ICategoryServices _categoryServices;

        public VehicleController(IVehicleServices vehicleServices, ICategoryServices categoryServices)
        {
            _vehicleServices = vehicleServices;
            _categoryServices = categoryServices;
        }

        public async Task<IActionResult> IndexVehicle()
        {
            var vehicles = await _vehicleServices.GetAllVehicle();
            return View("IndexVehicle", vehicles);
        }

        [HttpGet("GetAdminVehicle")]
        public async Task<IActionResult> GetAdminVehicle()
        {
            try
            {
                var vehicleList = await _vehicleServices.GetVehiclesByInventoryIdAsync(2);
                return Json(vehicleList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("UpdateVehicle")]
        public async Task<IActionResult> UpdateVehicle([FromBody] UpdateVehicleInventoryRequest request)
        {
            try
            {
                await _vehicleServices.UpdateVehicleInventoryAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteVehicleInventory/{id}")]
        public async Task<IActionResult> DeleteVehicleInventory(int id)
        {
            try
            {
                await _vehicleServices.DeleteVehicleInventoryAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("detail/{vehicleId}")]
        public async Task<IActionResult> VehicleDetail(int vehicleId)
        {
            var vehicle = await _vehicleServices.GetVehicleById(vehicleId);
            return View("VehicleDetail", vehicle);
        }

        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> VehicleDealer(int dealerId)
        {
            var vehicle = await _vehicleServices.GetVehicleByDealerId(dealerId);
            return View("VehicleDealer", vehicle);
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddVehicle()
        {
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();
            return View("AddVehicle");
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(CreateVehicleRequest request)
        {
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            var imageFile = Request.Form.Files["ImageFile"];
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/vehicles");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                request.Image = "/images/vehicles/" + fileName;
            }

            if (!ModelState.IsValid)
            {
                return View("AddVehicle", request);
            }
            var result = await _vehicleServices.AddVehicle(request);
            if (result)
                return RedirectToAction("IndexVehicle");
            ModelState.AddModelError("", "Failed to add vehicle.");
            return View("AddVehicle", request);
        }

        [HttpGet("update/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(int vehicleId)
        {
            var vehicle = await _vehicleServices.GetVehicleById(vehicleId);
            if (vehicle == null)
                return NotFound();
            var categories = await _categoryServices.GetAllCategory();
            var updateRequest = new UpdateVehicleRequest
            {
                VehicleId = vehicle.VehicleId,
                CategoryId = categories.FirstOrDefault(c => c.Name == vehicle.Category)?.CategoryId ?? 0,
                Color = vehicle.Color,
                Price = vehicle.Price,
                ManufactureDate = vehicle.ManufactureDate,
                Model = vehicle.Model,
                Version = vehicle.Version,
                Image = vehicle.Image
            };
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();
            return View("UpdateVehicle", updateRequest);
        }

        [HttpPost("update/{vehicleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, UpdateVehicleRequest request)
        {
            request.VehicleId = vehicleId;
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            var imageFile = Request.Form.Files["ImageFile"];
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/vehicles");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                request.Image = "/images/vehicles/" + fileName;
            }

            if (request.VehicleId <= 0)
            {
                ModelState.AddModelError("", "Invalid vehicle ID.");
                return View("UpdateVehicle", request);
            }
            if (!ModelState.IsValid)
            {
                return View("UpdateVehicle", request);
            }
            var result = await _vehicleServices.UpdateVehicle(vehicleId, request);
            if (result)
                return RedirectToAction("IndexVehicle");
            ModelState.AddModelError("", "Failed to update vehicle.");
            return View("UpdateVehicle", request);
        }

        [HttpPost("ImportVehicles")]
        [RequestSizeLimit(2 * 1024 * 1024)] // 2MB limit
        [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
        public async Task<IActionResult> ImportVehicles(IFormFile file)
        {
            Console.WriteLine($"📤 ImportVehicles called with file: {file?.FileName ?? "null"}");
            
            try
            {
                // Validate request
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("❌ No file provided");
                    return BadRequest(new { success = false, message = "No file provided" });
                }

                Console.WriteLine($"📊 File details: Name={file.FileName}, Size={file.Length} bytes, Type={file.ContentType}");

                // Validate file size
                if (file.Length > 2 * 1024 * 1024) // 2MB
                {
                    Console.WriteLine($"❌ File too large: {file.Length} bytes");
                    return BadRequest(new { success = false, message = "File size exceeds 2MB limit" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                Console.WriteLine($"📋 File extension: {fileExtension}");
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine($"❌ Invalid file format: {fileExtension}");
                    return BadRequest(new { success = false, message = "Invalid file format. Only CSV and Excel files are supported." });
                }

                Console.WriteLine("🔄 Starting file processing...");
                
                // Process the file with timeout protection
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                var result = await _vehicleServices.ImportVehiclesAsync(file);

                Console.WriteLine($"✅ Processing completed: Success={result.SuccessCount}, Errors={result.ErrorCount}");

                // Force garbage collection after processing
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Return standardized response
                return Ok(new
                {
                    success = true,
                    message = $"Import completed. {result.SuccessCount} vehicles imported successfully, {result.ErrorCount} errors occurred.",
                    successCount = result.SuccessCount,
                    errorCount = result.ErrorCount,
                    errors = result.Errors,
                    warnings = result.Warnings
                });
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("⏰ Import operation timed out");
                return StatusCode(408, new { success = false, message = "Import operation timed out. Please try with a smaller file." });
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine($"💾 Out of memory error: {ex.Message}");
                // Force cleanup on out of memory
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return StatusCode(413, new { success = false, message = "File too large to process. Please try with a smaller file." });
            }
            catch (IOException ex)
            {
                Console.WriteLine($"📁 File I/O error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error reading file. Please ensure the file is not corrupted and try again." });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"❌ ERROR: Import failed - {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"📍 Stack trace: {ex.StackTrace}");

                // Force cleanup on error
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return StatusCode(500, new { success = false, message = "An unexpected error occurred during import. Please try again." });
            }
        }
    }
}
