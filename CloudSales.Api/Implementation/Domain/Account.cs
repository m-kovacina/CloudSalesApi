namespace CloudSales.Api.Implementation.Domain
{
    public class Account
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public Customer Customer { get; set; }
        public List<PurchasedSoftware> PurchasedSoftware { get; set; }
    }
}
