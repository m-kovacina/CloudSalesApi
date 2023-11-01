using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;

namespace CloudSales.Api.Mappers
{
    public static class AccountMappers
    {
        public static AccountResponse MapToAccountDto(this Account account)
        {
            return new AccountResponse
            {
                Id = account.Id, 
                CustomerId = account.CustomerId, 
                Name = account.Name
            };
        }
    }
}
