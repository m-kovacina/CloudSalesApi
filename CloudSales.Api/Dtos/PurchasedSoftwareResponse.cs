using CloudSales.Api.Implementation.Domain;

namespace CloudSales.Api.Dtos
{
    public class PurchasedSoftwareResponse
    {
        public SoftwareService Software { get; set; }
        public int AccountId { get; set; }
        public string SubscriptionState { get; set; }
        public int Quantity { get; set; }
        public DateTime ValidToDateUtc { get; set; }
    }
}
