using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Services;
using Xunit;
using System.Threading.Tasks;

namespace SportsStore.Tests
{

    public class OrderControllerTests
    {

        private OrderController CreateController(IOrderRepository repo, Cart cart,
            bool paymentSuccess = true)
        {
            var loggerMock = new Mock<ILogger<OrderController>>();
            var configMock = new Mock<IConfiguration>();
            var paymentMock = new Mock<IPaymentService>();
            paymentMock.Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(),
                It.IsAny<string>()))
                .ReturnsAsync(new PaymentResult
                {
                    Success = paymentSuccess,
                    PaymentIntentId = "pi_test_123"
                });
            return new OrderController(repo, cart, loggerMock.Object,
                paymentMock.Object, configMock.Object);
        }

        [Fact]
        public async Task Cannot_Checkout_Empty_Cart()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            Order order = new Order();
            OrderController target = CreateController(mock.Object, cart);

            ViewResult? result = await target.Checkout(order, "pm_test") as ViewResult;

            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Cannot_Checkout_Invalid_ShippingDetails()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = CreateController(mock.Object, cart);
            target.ModelState.AddModelError("error", "error");

            ViewResult? result = await target.Checkout(new Order(), "pm_test") as ViewResult;

            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = CreateController(mock.Object, cart);

            RedirectToPageResult? result =
                await target.Checkout(new Order(), "pm_test") as RedirectToPageResult;

            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            Assert.Equal("/Completed", result?.PageName);
        }
    }
}