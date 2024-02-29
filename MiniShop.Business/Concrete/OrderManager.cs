using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MiniShop.Business.Abstract;
using MiniShop.Data.Abstract;
using MiniShop.Entity.Concrete;
using MiniShop.Entity.Concrete.Identity;
using MiniShop.Shared.ComplexTypes;
using MiniShop.Shared.ResponseViewModels;
using MiniShop.Shared.ViewModels;
using MiniShop.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderManager(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        public async Task<AdminOrderViewModel> GetOrderAsync(int orderId)
        {
            Order order = await _orderRepository.GetByIdAsync(x => x.Id == orderId, 
                source=> source.Include(x => x.OrderDetails).
                ThenInclude(y=>y.Product)); 
            var result = new AdminOrderViewModel
            {
                Id = order.Id,
                UserName = $"{order.FirstName} {order.LastName}",
                OrderDate = order.OrderDate,
                OrderDetails = order.OrderDetails.Select(od => new AdminOrderDetailViewModel
                {
                    Id = od.Id,
                    Price = od.Price,
                    Quantity = od.Quantity
                }).ToList(),
                UserId = order.UserId
            };
            return result;
        }

        public async Task<List<AdminOrderViewModel>> GetOrdersAsync()
        {
            List<Order> orders = await _orderRepository.GetAllAsync(null,
                source => source.Include(x => x.OrderDetails)
                .ThenInclude(y => y.Product)
                .Include(x => x.User));
                ;
            var result = orders.Select(o => new AdminOrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                UserName = $"{o.FirstName} {o.LastName}",
                State = o.OrderState.GetDisplayName(),
                OrderDetails = o.OrderDetails.Select(od => new AdminOrderDetailViewModel
                {
                    Id = od.Id,
                    Price = od.Price,
                    Quantity = od.Quantity
                }).ToList()
            }).ToList();
            return result.OrderByDescending(x=> x.Id).ToList();
        }

        public async Task<List<AdminOrderViewModel>> GetOrdersAsync(string userId)
        {
            List<Order> orders = await _orderRepository.GetAllAsync(x=>x.UserId==userId,
                source => source.Include(x => x.OrderDetails)
                .ThenInclude(y => y.Product));
            var result = orders.Select(o => new AdminOrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                UserName = $"{o.FirstName} {o.LastName}",
                OrderDetails = o.OrderDetails.Select(od => new AdminOrderDetailViewModel 
                { Id = od.Id, 
                  Price = od.Price, 
                  Quantity = od.Quantity,
                  Product = new ProductViewModel
                  {
                      Name = od.Product.Name,
                      ImageUrl = od.Product.ImageUrl
                  }
                  }).ToList()
            }).ToList();
            //Bu sıralama tercih edeceğimiz bir yöntem değil. Veri tabanından sıralı şekilde çekmeyi tercih edeceğiz.
            result = result.OrderByDescending(x => x.Id).ToList();
            return result;
        }

        public async Task<List<AdminOrderViewModel>> GetOrdersAsync(int productId)
        {
            List<Order> orders = await _orderRepository.GetAllOrdersByProductIdAsync(productId);
            var result = orders.Select(o => new AdminOrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                UserName = $"{o.FirstName} {o.LastName}",
                OrderDetails = o.OrderDetails.Select(od => new AdminOrderDetailViewModel { Id = od.Id, Price = od.Price, Quantity = od.Quantity }).ToList()
            }).ToList();
            //Bu sıralama tercih edeceğimiz bir yöntem değil. Veri tabanından sıralı şekilde çekmeyi tercih edeceğiz.
            result = result.OrderByDescending(x => x.Id).ToList();
            return result;
        }
        public async Task<OrderState> UpdateOrderStateAsync(int id, OrderState orderState)
        {
                var order = await _orderRepository.GetByIdAsync(x => x.Id == id);
                order.OrderState = orderState;
                await _orderRepository.UpdateAsync(order);
            return orderState;
        }
    }
}
