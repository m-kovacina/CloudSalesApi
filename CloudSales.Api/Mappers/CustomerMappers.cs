using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;

namespace CloudSales.Api.Mappers
{
    public static class CustomerMappers
    {
        public static CustomerResponse MapToCustomerDto(Customer  customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                CustomerIdentifier = customer.CustomerIdentifier,
                Name = customer.Name,
                Email = customer.Email,
                Type = customer.Type.ToString()
            };
        }
    }
}
