using CloudSales.Api.Data;
using CloudSales.Api.Implementation.Domain;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Implementation.Repositories
{
    public interface IAccountRepository{
        Task<List<Account>> GetAccountsByCustomer(string customerNumber);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly CloudSalesDbContext context;

        public AccountRepository(CloudSalesDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Account>> GetAccountsByCustomer(string customerNumber)
        {
            return await context.Accounts
                .Where(a => a.Customer.CustomerIdentifier == customerNumber).ToListAsync();
        }
    }
}
