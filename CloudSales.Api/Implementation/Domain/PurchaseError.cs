using Microsoft.AspNetCore.Mvc;

namespace CloudSales.Api.Implementation.Domain
{
    public interface IPurchaseError
    {
        public IActionResult ToActionResult();
    }

    public class ServiceNotFoundOnCppError : IPurchaseError
    {
        private readonly int serviceId;

        public ServiceNotFoundOnCppError(int serviceId)
        {
            this.serviceId = serviceId;
        }

        public IActionResult ToActionResult() => new NotFoundObjectResult(new ProblemDetails
        {
            Title = "SERVICE_NOT_FOUND",
            Detail = $"Service {serviceId} not found on CCP."
        });
    }

    public class SubscriptionAlreadyExistsError : IPurchaseError
    {
        private readonly int accountId;
        private readonly int serviceId;

        public SubscriptionAlreadyExistsError(int accountId, int serviceId)
        {
            this.accountId = accountId;
            this.serviceId = serviceId;
        }

        public IActionResult ToActionResult() => new UnprocessableEntityObjectResult(new ProblemDetails
        {
            Title = "SUBSCRIPTION_ALREADY_EXISTS",
            Detail = $"Subscription already exists for service: {serviceId} and account: {accountId}."
        });
    }

    public class InvalidSubscriptionDateError : IPurchaseError
    {
        public IActionResult ToActionResult() => new UnprocessableEntityObjectResult(new ProblemDetails
        {
            Title = "INVALID_SUBSCRIPTION_DATE",
            Detail = "Invalid subscription date"
        });
    }

    public class DbSaveChangesError : IPurchaseError
    {
        public IActionResult ToActionResult() => new UnprocessableEntityObjectResult(new ProblemDetails
        {
            Title = "FAILED_TO_SAVE_ORDER",
            Detail = "An error occurred while saving the record"
        });
    }
}
