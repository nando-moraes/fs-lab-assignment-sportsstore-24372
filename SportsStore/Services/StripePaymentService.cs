using Stripe;

namespace SportsStore.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly ILogger<StripePaymentService> logger;

        public StripePaymentService(IConfiguration config,
            ILogger<StripePaymentService> loggerService)
        {
            logger = loggerService;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }

        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount,
            string paymentMethodId)
        {
            try
            {
                logger.LogInformation("Fernando Moraes: Processing Stripe payment for amount {Amount}",
                    amount);

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "gbp",
                    PaymentMethod = paymentMethodId,
                    Confirm = true,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                        AllowRedirects = "never"
                    }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                logger.LogInformation("Fernando Moraes: Payment {PaymentIntentId} succeeded",
                    intent.Id);

                return new PaymentResult
                {
                    Success = true,
                    PaymentIntentId = intent.Id
                };

            }
            catch (StripeException ex)
            {
                logger.LogError(ex,
                    "Fernando Moraes: Stripe payment failed with error {ErrorMessage}",
                    ex.Message);
                return new PaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}