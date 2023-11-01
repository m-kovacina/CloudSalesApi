using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;

namespace CloudSales.Api.Mappers
{
    public static class PurchasedSoftwareMappers
    {
        public static List<PurchasedSoftwareResponse> MapToPurchasedSoftwareDtos(
            List<PurchasedSoftware> purchasedSoftwareList,
            List<SoftwareService> softwareServices)
        {
            return (from singlePurchasedSoftware in purchasedSoftwareList
                let matchingSoftwareService = softwareServices.FirstOrDefault(s => s.Id == singlePurchasedSoftware.ServiceId)
                select new PurchasedSoftwareResponse
                {
                    AccountId = singlePurchasedSoftware.AccountId,
                    Quantity = singlePurchasedSoftware.Quantity,
                    ValidToDateUtc = singlePurchasedSoftware.ValidToDateUtc,
                    SubscriptionState = singlePurchasedSoftware.State.ToString(),
                    Software = matchingSoftwareService
                }).ToList();
        }
    }
}
