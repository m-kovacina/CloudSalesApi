using CloudSales.Api.Implementation.Domain;

namespace CloudSales.Api.Implementation.Repositories
{
    public interface ICloudProviderRepository
    {
        Task<List<SoftwareService>> GetSoftwareServicesAsync();
    }

    public class CloudProviderRepository : ICloudProviderRepository
    {
        public Task<List<SoftwareService>> GetSoftwareServicesAsync()
        {
            var softwareServices = new List<SoftwareService>
            {
                new() {Id = 1, Name = "Microsoft Office", Description = "Office suite"},
                new() {Id = 2, Name = "Visual Studio", Description = "The most comprehensive IDE for .NET and C++ developers on Windows."},
                new() {Id = 3, Name = "Adobe Photoshop", Description = "Image editing software"}
            };

            return Task.FromResult(softwareServices);
        }
    }
}
