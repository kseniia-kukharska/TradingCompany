using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingCompanyBL.Interfaces;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using TradingCompanyWeb.Models;

namespace TradingCompanyWeb.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IOrderDal _orderDal;
        private readonly IStatusDal _statusDal;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderManager orderManager, IOrderDal orderDal, IStatusDal statusDal, IMapper mapper, ILogger<OrdersController> logger)
        {
            _orderManager = orderManager;
            _orderDal = orderDal;
            _statusDal = statusDal;
            _mapper = mapper;
            _logger = logger;
        }

        // 1. LIST
        public IActionResult Index()
        {
            var orders = _orderDal.GetAll();
            ViewBag.Statuses = _statusDal.GetAll();
            return View(_mapper.Map<IEnumerable<OrderViewModel>>(orders));
        }

        // 2. DETAILS
        public IActionResult Details(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();

            var status = _statusDal.GetById(order.StatusId);
            ViewBag.StatusName = status?.StatusName ?? "Unknown";

            return View(_mapper.Map<OrderViewModel>(order));
        }

        // 3. CREATE (GET)
        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            ViewBag.Statuses = _statusDal.GetAll();
            return View();
        }

        // 3. CREATE (POST)
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                _orderDal.Create(_mapper.Map<Order>(model));
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Statuses = _statusDal.GetAll();
            return View(model);
        }

        // 4. EDIT (GET)
        [Authorize(Roles = "Seller")]
        public IActionResult Edit(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();

            ViewBag.Statuses = _statusDal.GetAll();
            return View(_mapper.Map<OrderViewModel>(order));
        }

        // 4. EDIT (POST)
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, OrderViewModel model)
        {
            if (id != model.OrderId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var order = _mapper.Map<Order>(model);
                    int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                    _orderManager.UpdateOrderWithHistory(order, userId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Update failed: {Message}", ex.Message);
                    ModelState.AddModelError("", "Unable to save changes. Format error or database constraint.");
                }
            }
            ViewBag.Statuses = _statusDal.GetAll();
            return View(model);
        }

        // 5. DELETE (GET - Confirmation Page)
        [Authorize(Roles = "Seller")]
        public IActionResult Delete(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();

            var status = _statusDal.GetById(order.StatusId);
            ViewBag.StatusName = status?.StatusName ?? "Unknown";

            return View(_mapper.Map<OrderViewModel>(order));
        }

        // 5. DELETE (POST - Execution)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderDal.Delete(id);
            _logger.LogInformation("Order {Id} deleted", id);
            return RedirectToAction(nameof(Index));
        }
    }
}