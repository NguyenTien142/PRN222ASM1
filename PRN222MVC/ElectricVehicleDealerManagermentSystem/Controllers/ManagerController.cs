using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;
using BusinessObject.BusinessObject.OrderModels.Respond;
using BusinessObject.BusinessObject.OrderModels.Response;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IOrderServices _orderServices;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(IOrderServices orderServices, ILogger<ManagerController> logger)
        {
            _orderServices = orderServices;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "Dealer Manager";

                // Get dashboard statistics
                var pendingOrders = await _orderServices.GetAllPendingOrdersAsync();
                var pendingCount = pendingOrders.Count;
                var totalValue = pendingOrders.Sum(o => o.TotalAmount);

                // Pass statistics to view
                ViewBag.PendingOrdersCount = pendingCount;
                ViewBag.TotalPendingValue = totalValue;

                return View("ManagerIndex");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading manager dashboard");

                // Fallback values if error occurs
                ViewBag.Username = GetCurrentUsername() ?? "Dealer Manager";
                ViewBag.PendingOrdersCount = 0;
                ViewBag.TotalPendingValue = 0;

                return View("ManagerIndex");
            }
        }

        // Order Approval Management
        public async Task<IActionResult> OrderApproval()
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "Dealer Manager";

                // Get all pending orders for approval
                var pendingOrders = await _orderServices.GetAllPendingOrdersAsync();
                ViewBag.PendingOrders = pendingOrders;

                return View("OrderApproval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order approval page");

                // Fallback values if error occurs
                ViewBag.Username = GetCurrentUsername() ?? "Dealer Manager";
                ViewBag.PendingOrders = new List<GetPendingOrderResponse>();

                return View("OrderApproval");
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdForApproval(id);
                if (order == null)
                {
                    return NotFound();
                }

                // Check if request is AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(order);
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order details for approval");
                return Json(new { success = false, message = "Error loading order details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            try
            {
                var result = await _orderServices.ApproveOrderAsync(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = result,
                        message = result ? "Order approved successfully" : "Failed to approve order"
                    });
                }

                TempData["Success"] = result ? "Order approved successfully" : "Failed to approve order";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order {OrderId}", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while approving the order" });
                }

                TempData["Error"] = "An error occurred while approving the order";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(OrderApproval));
        }

        [HttpPost]
        public async Task<IActionResult> RejectOrder(int id)
        {
            try
            {
                var result = await _orderServices.RejectOrderAsync(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = result,
                        message = result ? "Order rejected successfully" : "Failed to reject order"
                    });
                }

                TempData["Success"] = result ? "Order rejected successfully" : "Failed to reject order";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order {OrderId}", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while rejecting the order" });
                }

                TempData["Error"] = "An error occurred while rejecting the order";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(OrderApproval));
        }

        // Helper method to get current username from JWT token
        private string GetCurrentUsername()
        {
            var token = Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    return jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                }
            }
            catch
            {
                // Token parsing failed
            }

            return null;
        }
    }
}
