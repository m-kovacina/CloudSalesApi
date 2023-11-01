using CloudSales.Api.Data;
using CloudSales.Api.Implementation.Domain;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Implementation.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByNumber(string customerNumber);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly CloudSalesDbContext context;

        public CustomerRepository(CloudSalesDbContext context)
        {
            this.context = context;
        }

        public async Task<Customer> GetCustomerByNumber(string customerNumber)
        {
            return await context.Customers
                .SingleOrDefaultAsync(c => c.CustomerIdentifier == customerNumber);
        }
    }
}
