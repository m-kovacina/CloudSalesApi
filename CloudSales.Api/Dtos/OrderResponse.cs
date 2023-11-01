using Microsoft.AspNetCore.Mvc;

namespace CloudSales.Api.Dtos
{
    public class OrderResponse
    {
        public bool Success { get; set; }
        public SubscriptionResponse? SubscriptionResponse { get; set; }
        public IActionResult? OrderError { get; set; }
    }

    public class SubscriptionResponse
    {
        public int ServiceId { get; set; }
        public int AccountId { get; set; }
        public int Quantity { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
