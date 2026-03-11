using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{

    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderRepository repoService, Cart cartService,
            ILogger<OrderController> loggerService)
        {
            repository = repoService;
            cart = cartService;
            logger = loggerService;
        }

        public ViewResult Checkout()
        {
            logger.LogInformation("Fernando Moraes: Customer started checkout with {ItemCount} items in cart",
                cart.Lines.Count());
            return View(new Order());
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                logger.LogWarning("Fernando Moraes: Checkout attempted with empty cart");
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.Lines.ToArray();
                repository.SaveOrder(order);
                cart.Clear();
                logger.LogInformation("Fernando Moraes: Order {OrderId} created for {CustomerName} shipping to {City}",
                    order.OrderID, order.Name, order.City);
                return RedirectToPage("/Completed", new { orderId = order.OrderID });
            }
            else
            {
                logger.LogWarning("Fernando Moraes: Checkout validation failed for {CustomerName}",
                    order.Name ?? "unknown");
                return View();
            }
        }
    }
}