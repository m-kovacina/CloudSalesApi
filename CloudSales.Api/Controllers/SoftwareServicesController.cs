using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;
using CloudSales.Api.Implementation.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CloudSales.Api.Controllers
{
    [Route("api/software-services")]
    [ApiController]
    public class SoftwareServicesController : ControllerBase
    {
        private readonly ICloudProviderRepository cloudProviderRepository;
        
        public SoftwareServicesController(ICloudProviderRepository cloudProviderRepository)
        {
            this.cloudProviderRepository = cloudProviderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Response<List<SoftwareService>>>> GetSoftwareServices()
        {
            try
            {
                // Mocked data - replace this with actual data from CCP
                var softwareServices = await cloudProviderRepository.GetSoftwareServicesAsync();

                return Ok(new Response<List<SoftwareService>>(softwareServices));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
