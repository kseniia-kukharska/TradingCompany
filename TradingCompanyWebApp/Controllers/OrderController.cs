using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TradingCompanyBL.Interfaces;
using TradingCompanyWebApp.Models;
using System.Security.Claims;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderManager _orderManager;
    private readonly IMapper _mapper;

    public OrderController(IOrderManager orderManager, IMapper mapper)
    {
        _orderManager = orderManager;
        _mapper = mapper;
    }

    // Тільки користувачі з RoleId = 4 зможуть побачити цей список
    [Authorize(Roles = "4")]
    public IActionResult Index()
    {
        var orders = _orderManager.GetFilteredOrders(null, null, null);
        var models = _mapper.Map<IEnumerable<OrderViewModel>>(orders);
        return View(models);
    }
}