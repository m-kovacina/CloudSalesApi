using System.ComponentModel.DataAnnotations;

namespace CloudSales.Api.Implementation.Domain
{
    public class Customer
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string CustomerIdentifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public CustomerType Type { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public enum CustomerType
    {
        Direct = 1,
        ReSeller = 2
    }
}
