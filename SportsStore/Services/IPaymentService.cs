namespace SportsStore.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(decimal amount, string paymentMethodId);
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}