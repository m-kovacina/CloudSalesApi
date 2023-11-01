namespace CloudSales.Api.Dtos
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string CustomerIdentifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
    }
}
