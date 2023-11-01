namespace CloudSales.Api.Dtos
{
    public class OrderRequest
    {
        public int ServiceId { get; set; }
        public int Quantity { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
