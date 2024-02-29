using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniShop.Business.Abstract;
using MiniShop.Shared.ViewModels;

namespace MiniShop.UI.Areas.Admin.Controllers
{
    [Authorize(Roles ="SuperAdmin, Admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IOrderService _orderManager;

        public HomeController(IOrderService orderManager)
        {
            _orderManager = orderManager;
        }

        public async Task<IActionResult> Index()
        {
            List<AdminOrderViewModel> orders = await _orderManager.GetOrdersAsync();
            orders = orders.Take(2).ToList();
            return View(orders);
        }
        public async Task<IActionResult> FilterOrders(int id)
        {
            List<AdminOrderViewModel> orders = await _orderManager.GetOrdersAsync();
            orders = orders.Take(id).ToList();
            return PartialView("_FilterByNumberPartial", orders);
        }
    }
}
