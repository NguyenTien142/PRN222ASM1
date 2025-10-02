using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessObject.BusinessObject.OrderModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Intefaces;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderServices _orderService;
        private readonly IVehicleServices _vehicleServices;
        private readonly ICustomerService _customerService;
        public OrderController(IOrderServices orderService, IVehicleServices vehicleServices, ICustomerService customerService)
        {
            _orderService = orderService;
            _vehicleServices = vehicleServices;
            _customerService = customerService;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromJwtCookie();
            if (userId == null)
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetAllOrders(userId.Value);
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserIdFromJwtCookie();
            if (userId == null)
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderById(userId.Value, id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            var vehicles = await _vehicleServices.GetAllVehicle();
            ViewBag.Vehicles = vehicles.Select(x => new SelectListItem
            {
                Value = x.VehicleId.ToString(),
                Text = x.Model
            }).ToList();
            var customers = await _customerService.GetAllCustomers();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderRequestDto model)
        {
            var userId = GetUserIdFromJwtCookie();
            if (userId == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var requestModel = new CreateOrderRequestDto
                {
                    CustomerId = model.CustomerId,
                    OrderVehicles = model.OrderVehicles.Where(ov => ov.VehicleId > 0).ToList()
                };
                await _orderService.CreateOrder(userId.Value, requestModel);
            }
            var vehicles = await _vehicleServices.GetAllVehicle();

            ViewBag.Vehicles = vehicles.Select(x => new SelectListItem
            {
                Value = x.VehicleId.ToString(),
                Text = x.Model
            }).ToList();
            var customers = await _customerService.GetAllCustomers();
            ViewBag.CustomerId = new SelectList(customers, "CustomerId", "Name");
            return View(model);
        }
        //
        // // GET: Order/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var order = await _context.Orders.FindAsync(id);
        //     if (order == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", order.CustomerId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "UserId", "PasswordHash", order.UserId);
        //     return View(order);
        // }
        //
        // // POST: Order/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,UserId,OrderDate,TotalAmount,Status")] Order order)
        // {
        //     if (id != order.OrderId)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(order);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!OrderExists(order.OrderId))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", order.CustomerId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "UserId", "PasswordHash", order.UserId);
        //     return View(order);
        // }
        //
        // // GET: Order/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var order = await _context.Orders
        //         .Include(o => o.Customer)
        //         .Include(o => o.User)
        //         .FirstOrDefaultAsync(m => m.OrderId == id);
        //     if (order == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(order);
        // }
        //
        // // POST: Order/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var order = await _context.Orders.FindAsync(id);
        //     if (order != null)
        //     {
        //         _context.Orders.Remove(order);
        //     }
        //
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
        //
        // private bool OrderExists(int id)
        // {
        //     return _context.Orders.Any(e => e.OrderId == id);
        // }

        private int? GetUserIdFromJwtCookie()
        {
            var token = Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
