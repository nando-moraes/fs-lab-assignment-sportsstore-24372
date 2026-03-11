using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Services;

namespace SportsStore.Controllers
{

    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private readonly ILogger<OrderController> logger;
        private readonly IPaymentService paymentService;
        private readonly IConfiguration config;

        public OrderController(IOrderRepository repoService, Cart cartService,
            ILogger<OrderController> loggerService, IPaymentService paymentService,
            IConfiguration configuration)
        {
            repository = repoService;
            cart = cartService;
            logger = loggerService;
            this.paymentService = paymentService;
            config = configuration;
        }

        public ViewResult Checkout()
        {
            logger.LogInformation("Fernando Moraes: Customer started checkout with {ItemCount} items in cart",
                cart.Lines.Count());
            ViewBag.StripePublishableKey = config["Stripe:PublishableKey"];
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string paymentMethodId)
        {
            if (cart.Lines.Count() == 0)
            {
                logger.LogWarning("Fernando Moraes: Checkout attempted with empty cart");
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            if (ModelState.IsValid)
            {
                decimal total = cart.Lines.Sum(l => l.Product.Price * l.Quantity);
                var paymentResult = await paymentService.ProcessPaymentAsync(total, paymentMethodId);

                if (paymentResult.Success)
                {
                    order.Lines = cart.Lines.ToArray();
                    order.PaymentIntentId = paymentResult.PaymentIntentId;
                    order.PaymentComplete = true;
                    repository.SaveOrder(order);
                    cart.Clear();
                    logger.LogInformation("Fernando Moraes: Order {OrderId} created for {CustomerName}, payment {PaymentIntentId}",
                        order.OrderID, order.Name, order.PaymentIntentId);
                    return RedirectToPage("/Completed", new { orderId = order.OrderID });
                }
                else
                {
                    logger.LogError("Fernando Moraes: Payment failed for {CustomerName} - {ErrorMessage}",
                        order.Name, paymentResult.ErrorMessage);
                    ModelState.AddModelError("", $"Payment failed: {paymentResult.ErrorMessage}");
                    ViewBag.StripePublishableKey = config["Stripe:PublishableKey"];
                    return View(order);
                }
            }
            else
            {
                logger.LogWarning("Fernando Moraes: Checkout validation failed for {CustomerName}",
                    order.Name ?? "unknown");
                ViewBag.StripePublishableKey = config["Stripe:PublishableKey"];
                return View(order);
            }
        }
    }
}