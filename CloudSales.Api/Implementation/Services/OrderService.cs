using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;
using CloudSales.Api.Implementation.Repositories;
using CloudSales.Api.Implementation.Validators;
using FluentValidation.Results;

namespace CloudSales.Api.Implementation.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> PlaceOrderAsync(int accountId, OrderRequest orderRequest);
        Task<ValidationResult> ValidateOrderRequestAsync(OrderRequest orderRequest);
    }

    public class OrderService : IOrderService
    {
        private readonly IPurchasedSoftwareRepository purchasedSoftwareRepository;
        private readonly ICloudProviderRepository cloudProviderRepository;
        
        public OrderService(IPurchasedSoftwareRepository purchasedSoftwareRepository,
            ICloudProviderRepository cloudProviderRepository)
        {
            this.purchasedSoftwareRepository = purchasedSoftwareRepository;
            this.cloudProviderRepository = cloudProviderRepository;
        }

        public async Task<OrderResponse> PlaceOrderAsync(int accountId, OrderRequest orderRequest)
        {
            if (!await CheckServiceExistsOnCCPAsync(orderRequest.ServiceId))
            {
                return new OrderResponse
                {
                    Success = false,
                    OrderError = new ServiceNotFoundOnCppError(orderRequest.ServiceId).ToActionResult()
                };
            }

            if (await CheckSubscriptionExistsAsync(accountId, orderRequest.ServiceId))
            {
                return new OrderResponse
                {
                    Success = false,
                    OrderError = new SubscriptionAlreadyExistsError(accountId, orderRequest.ServiceId).ToActionResult()
                };
            }

            if (orderRequest.ValidTo <= DateTime.UtcNow)
            {
                return new OrderResponse
                {
                    Success = false,
                    OrderError = new InvalidSubscriptionDateError().ToActionResult()
                };
            }

            var orderResponse = await PlaceSoftwareOrderAsync(accountId, orderRequest);

            return orderResponse
                ? new OrderResponse
                {
                    Success = true,
                    SubscriptionResponse = new SubscriptionResponse
                    {
                        AccountId = accountId, 
                        Quantity = orderRequest.Quantity, 
                        ServiceId = orderRequest.ServiceId,
                        ValidTo = orderRequest.ValidTo.ToUniversalTime()
                    }
                }
                : new OrderResponse {Success = false, OrderError = new DbSaveChangesError().ToActionResult()};
        }

        public async Task<ValidationResult> ValidateOrderRequestAsync(OrderRequest orderRequest)
        {
            var validator = new OrderRequestValidator();
            return await validator.ValidateAsync(orderRequest);
        }

        private async Task<bool> CheckServiceExistsOnCCPAsync(int serviceId)
        {
            var softwareServices = await cloudProviderRepository.GetSoftwareServicesAsync();
            return softwareServices.Any(s => s.Id == serviceId);
        }

        private async Task<bool> CheckSubscriptionExistsAsync(int accountId, int serviceId)
        {
            return await purchasedSoftwareRepository.SubscriptionExists(accountId, serviceId);
        }

        private async Task<bool> PlaceSoftwareOrderAsync(int accountId, OrderRequest orderRequest)
        {
            return await purchasedSoftwareRepository.PlaceOrder(new PurchasedSoftware
            {
                AccountId = accountId,
                Quantity = orderRequest.Quantity,
                ServiceId = orderRequest.ServiceId,
                State = PurchasedState.Active,
                ValidToDateUtc = orderRequest.ValidTo.ToUniversalTime()
            });
        }
    }
}
