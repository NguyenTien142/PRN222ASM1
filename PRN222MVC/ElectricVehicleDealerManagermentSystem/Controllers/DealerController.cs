using BusinessObject.BusinessObject.DealerModels.Respond;
using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class DealerController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;

        public DealerController(IOrderService orderService, IInventoryService inventoryService)
        {
            _orderService = orderService;
            _inventoryService = inventoryService;
        }


        public IActionResult Dashboard()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var dealerId = HttpContext.Session.GetInt32("DealerId");

                Console.WriteLine($"DEBUG: UserId = {userId}, DealerId = {dealerId}");

                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }

                if (!dealerId.HasValue)
                {
                    return Json(new { success = false, message = "Dealer information not found" });
                }

                var successfulSummary = await _orderService.GetSuccessfulOrderAsync(userId.Value);
                var pendingSummary = await _orderService.GetPendingOrderAsync(userId.Value);
                var stockQuantity = await _inventoryService.GetTotalStockQuantityByDealerAsync(dealerId.Value);
                var totalEarnings = await _orderService.GetTotalEarningsByUserAsync(userId.Value);

                var stats = new
                {
                    successfulOrdersCount = successfulSummary.Count,
                    pendingOrdersCount = pendingSummary.Count,
                    stockQuantity = stockQuantity,
                    totalEarnings = totalEarnings,
                    totalSales = successfulSummary.Count + pendingSummary.Count
                };

                Console.WriteLine($"DEBUG: Returning stats = {System.Text.Json.JsonSerializer.Serialize(stats)}");

                return Json(stats); // Direct JSON response
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSuccessfulOrders()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }
                var successfulOrders = await _orderService.GetSuccessfulOrderAsync(userId.Value);
                
                return Json(successfulOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
