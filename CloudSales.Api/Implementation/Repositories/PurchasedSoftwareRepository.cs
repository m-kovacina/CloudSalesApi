using CloudSales.Api.Data;
using CloudSales.Api.Implementation.Domain;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Implementation.Repositories
{
    public interface IPurchasedSoftwareRepository
    {        
        Task<bool> PlaceOrder(PurchasedSoftware purchasedSoftware);
        Task<bool> SubscriptionExists(int accountId, int serviceId);
        Task<List<PurchasedSoftware>> GetPurchasedSoftwareForAccount(int accountId);
        Task<bool> UpdateSubscriptionQuantity(int accountId, int serviceId, int quantity);
        Task<bool> CancelSubscription(int accountId, int serviceId);
        Task<bool> ExtendLicenseValidity(int accountId, int serviceId, DateTime newValidToDateUtc);
    }

    public class PurchasedSoftwareRepository : IPurchasedSoftwareRepository
    {
        private readonly CloudSalesDbContext dataContext;

        public PurchasedSoftwareRepository(CloudSalesDbContext context)
        {
            this.dataContext = context;
        }

        public async Task<bool> PlaceOrder(PurchasedSoftware purchasedSoftware)
        {
            await dataContext.PurchasedSoftware.AddAsync(purchasedSoftware);
            var created = await dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> SubscriptionExists(int accountId, int serviceId)
        {
            return await dataContext.PurchasedSoftware
                .AnyAsync(p => p.AccountId == accountId && p.ServiceId == serviceId);
        }

        public async Task<List<PurchasedSoftware>> GetPurchasedSoftwareForAccount(int accountId)
        {
            return await dataContext.PurchasedSoftware
                .Where(p => p.AccountId == accountId).ToListAsync();
        }

        public async Task<bool> UpdateSubscriptionQuantity(int accountId, int serviceId, int newQuantity)
        {
            var existingSubscription = await dataContext.PurchasedSoftware.AsNoTracking()
                .SingleOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId);
            
            if (existingSubscription == null)
                return false;

            existingSubscription.Quantity = newQuantity;
            dataContext.PurchasedSoftware.Update(existingSubscription);
            var updated = await dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> CancelSubscription(int accountId, int serviceId)
        {
            var existingSubscription = await dataContext.PurchasedSoftware.AsNoTracking()
                .SingleOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId);
            
            if (existingSubscription == null)
                return false;

            existingSubscription.State = PurchasedState.Cancelled;
            dataContext.PurchasedSoftware.Update(existingSubscription);
            var updated = await dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> ExtendLicenseValidity(int accountId, int serviceId, DateTime newValidToDateUtc)
        {
            var existingSubscription = await dataContext.PurchasedSoftware.AsNoTracking()
                .SingleOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId);
            
            if (existingSubscription == null)
                return false;

            existingSubscription.ValidToDateUtc = newValidToDateUtc;
            dataContext.PurchasedSoftware.Update(existingSubscription);
            var updated = await dataContext.SaveChangesAsync();
            return updated > 0;
        }
    }
}
