using CloudSales.Api.Dtos;
using FluentValidation;

namespace CloudSales.Api.Implementation.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(orderRequest => orderRequest.ServiceId)
                .NotEmpty()
                .WithMessage("ServiceID is required.");

            RuleFor(orderRequest => orderRequest.Quantity)
                .NotEmpty()
                .WithMessage("Quantity is required.")
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
        }
    }
}
