using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.Business.Abstract;
using MiniShop.Entity.Concrete;
using MiniShop.Shared.ComplexTypes;
using MiniShop.Shared.ViewModels;
using MiniShop.UI.Areas.Admin.Models;
using MiniShop.UI.Extensions;

namespace MiniShop.UI.Areas.Admin.Controllers
{
    [Authorize(Roles ="SuperAdmin, Admin")]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderManager;
        private readonly IProductService _productManager;

        public OrderController(IOrderService orderManager, IProductService productManager)
        {
            _orderManager = orderManager;
            _productManager = productManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderManager.GetOrdersAsync();
            var result = await _productManager.GetAllNonDeletedAsync();
            var products = result.Data;
            List<SelectListItem> productListItems = products.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),

            }).ToList();
            OrderFilterViewModel model = new OrderFilterViewModel { Orders= orders, ProductListItems = productListItems };
            return View(model);
        }
        public async Task<IActionResult> FilterByProduct(int id)
        {
            List<AdminOrderViewModel> orders;
            if(id == null)
            {
                orders = await _orderManager.GetOrdersAsync();

                return PartialView("_FilterByProductPartial", orders);
            }
            orders = await _orderManager.GetOrdersAsync(id);
            
            return PartialView("_FilterByProductPartial", orders);
        }
        public async Task<IActionResult> ChangeState(int id)
        {
            var orderState = await _orderManager.UpdateOrderStateAsync(id, OrderState.Preparing);
            return Json(orderState.GetDisplayName());
        }
    }
}
