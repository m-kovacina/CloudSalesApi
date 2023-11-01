namespace CloudSales.Api.Implementation.Domain
{
    public class PurchasedSoftware
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
        public PurchasedState State { get; set; }
        public DateTime ValidToDateUtc { get; set; }
        public Account Account { get; set; }
    }

    public enum PurchasedState
    {
        Active = 1,
        Expired = 2,
        Cancelled = 3,
        Pending = 4
    };
}
