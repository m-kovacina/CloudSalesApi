using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Repositories;
using CloudSales.Api.Implementation.Services;
using CloudSales.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace CloudSales.Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IPurchasedSoftwareRepository purchasedSoftwareRepository;
        private readonly IOrderService orderService;
        private readonly ICloudProviderRepository cloudProviderRepository;

        public AccountsController(IPurchasedSoftwareRepository purchasedSoftwareRepository, IOrderService orderService,
            ICloudProviderRepository cloudProviderRepository)
        {
            this.purchasedSoftwareRepository = purchasedSoftwareRepository;
            this.orderService = orderService;
            this.cloudProviderRepository = cloudProviderRepository;
        }

        [HttpPost("{accountId}/order")]
        public async Task<IActionResult> PlaceOrder([FromRoute] int accountId, [FromBody] OrderRequest orderRequest)
        {
            var validationResult = await orderService.ValidateOrderRequestAsync(orderRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var orderResult = await orderService.PlaceOrderAsync(accountId, orderRequest);
                return orderResult.Success ? Ok(orderResult.SubscriptionResponse) : orderResult.OrderError;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }

        [HttpGet("{accountId}/purchased-software")]
        public async Task<IActionResult> GetPurchasedSoftwareForAccount(int accountId)
        {
            try
            {
                var purchasedSoftware = await purchasedSoftwareRepository.GetPurchasedSoftwareForAccount(accountId);
                if (purchasedSoftware == null)
                {
                    return NotFound();
                }

                var softwareServices = await cloudProviderRepository.GetSoftwareServicesAsync();
                var result = PurchasedSoftwareMappers.MapToPurchasedSoftwareDtos(purchasedSoftware, softwareServices);
                return Ok(new Response<List<PurchasedSoftwareResponse>>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{accountId}/subscriptions/{serviceId}")]
        public async Task<IActionResult> ChangeSubscriptionQuantity(int accountId, int serviceId, [FromBody] ChangeQuantityRequest changeRequest)
        {
            try
            {
                if (!await purchasedSoftwareRepository.SubscriptionExists(accountId, serviceId))
                {
                    return NotFound("Subscription not found.");
                }

                if (changeRequest.NewQuantity < 1)
                {
                    return BadRequest("Invalid quantity.");
                }

                var success =
                    await purchasedSoftwareRepository.UpdateSubscriptionQuantity(accountId, serviceId,
                        changeRequest.NewQuantity);

                return success
                    ? Ok("Quantity updated successfully.")
                    : StatusCode(StatusCodes.Status500InternalServerError, "Failed to update quantity.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{accountId}/subscriptions/{serviceId}")]
        public async Task<IActionResult> CancelSubscription(int accountId, int serviceId)
        {
            try
            {
                if (!await purchasedSoftwareRepository.SubscriptionExists(accountId, serviceId))
                {
                    return NotFound("Subscription not found.");
                }

                var success = await purchasedSoftwareRepository.CancelSubscription(accountId, serviceId);

                return success
                    ? Ok("Subscription canceled successfully.")
                    : StatusCode(StatusCodes.Status500InternalServerError, "Failed to cancel subscription.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{accountId}/subscriptions/{serviceId}/extend")]
        public async Task<IActionResult> ExtendLicenseValidity(int accountId, int serviceId,
            [FromBody] ExtensionRequest extensionRequest)
        {
            try
            {
                if (!await purchasedSoftwareRepository.SubscriptionExists(accountId, serviceId))
                {
                    return NotFound("Subscription not found.");
                }

                if (extensionRequest.NewValidToDate <= DateTime.UtcNow)
                {
                    return BadRequest("Invalid subscription date.");
                }

                var newValidToDateUtc = extensionRequest.NewValidToDate.ToUniversalTime();

                var success =
                    await purchasedSoftwareRepository.ExtendLicenseValidity(accountId, serviceId, newValidToDateUtc);

                return success
                    ? Ok("License validity extended successfully.")
                    : StatusCode(StatusCodes.Status500InternalServerError, "Failed to extend license validity.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
