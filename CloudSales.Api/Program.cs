using CloudSales.Api.Data;
using CloudSales.Api.Filters;
using CloudSales.Api.Implementation.Repositories;
using CloudSales.Api.Implementation.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace CloudSales.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<CloudSalesDbContext>(opt => opt.UseSqlServer(
                builder.Configuration.GetConnectionString("CloudSalesConnection")));

            // Add services to the container.

            builder.Services.AddControllers(options =>
                options.Filters.Add<ApiKeyAttribute>());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Api Key Auth", Version = "v1"});
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "ApiKey must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-Api-Key",
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    {key, new List<string>()}
                };
                c.AddSecurityRequirement(requirement);
            });

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICloudProviderRepository, CloudProviderRepository>();
            builder.Services.AddScoped<IPurchasedSoftwareRepository, PurchasedSoftwareRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
        }
    }
}