using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Repositories;
using CloudSales.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace CloudSales.Api.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly ICustomerRepository customerRepository;

        public CustomerController(IAccountRepository accountRepository, ICustomerRepository customerRepository)
        {
            this.accountRepository = accountRepository;
            this.customerRepository = customerRepository;
        }

        [HttpGet("{customerNumber}")]
        public async Task<IActionResult> GetCustomerByNumber(string customerNumber)
        {
            var customer = await customerRepository.GetCustomerByNumber(customerNumber);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(CustomerMappers.MapToCustomerDto(customer));
        }

        [HttpGet("{customerNumber}/accounts")]
        public async Task<IActionResult> GetAccountsByCustomerNumber(string customerNumber)
        {
            try
            {
                var accounts = await accountRepository.GetAccountsByCustomer(customerNumber);
                return Ok(new Response<List<AccountResponse>>(accounts.Select(a => a.MapToAccountDto()).ToList()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
