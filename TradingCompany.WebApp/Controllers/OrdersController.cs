using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var orders = _orderDal.GetAll();
            ViewBag.Statuses = _statusDal.GetAll();
            return View(_mapper.Map<IEnumerable<OrderViewModel>>(orders));
        }

        public IActionResult Details(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();
            ViewBag.StatusName = _statusDal.GetById(order.StatusId)?.StatusName ?? "Unknown";
            return View(_mapper.Map<OrderViewModel>(order));
        }

        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            ViewBag.Statuses = _statusDal.GetAll();
            return View();
        }

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

        [Authorize(Roles = "Seller")]
        public IActionResult Edit(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();
            ViewBag.Statuses = _statusDal.GetAll();
            return View(_mapper.Map<OrderViewModel>(order));
        }

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
                    _orderManager.UpdateOrder(order, userId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Error updating order {OrderId}", id);
                    ModelState.AddModelError("", "Database error occurred while saving.");
                }
            }
            ViewBag.Statuses = _statusDal.GetAll();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        public IActionResult Delete(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null) return NotFound();
            return View(_mapper.Map<OrderViewModel>(order));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderDal.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}