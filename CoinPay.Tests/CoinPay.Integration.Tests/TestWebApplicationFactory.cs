using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CoinPay.Api.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using CoinPay.Api.Services.Circle;

namespace CoinPay.Integration.Tests;

/// <summary>
/// Custom WebApplicationFactory for integration testing
/// Uses in-memory database to isolate tests
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Override configuration for testing
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Disable Vault for testing
                {"Vault:Enabled", "false"},
                {"Vault:Address", "http://localhost:8200"},
                {"Vault:Token", "test-token"},
                // Add required JWT configuration
                {"Jwt:Issuer", "CoinPayTestIssuer"},
                {"Jwt:Audience", "CoinPayTestAudience"},
                {"Jwt:SecretKey", "ThisIsAVeryLongTestSecretKeyForJwtTokenGeneration12345678901234567890"},
                {"Jwt:ExpirationMinutes", "60"},
                // Disable Redis for testing
                {"Redis:Enabled", "false"},
                {"Redis:ConnectionString", "localhost:6379"},
                // Add Circle configuration (will be mocked)
                {"Circle:ApiKey", "test-api-key"},
                {"Circle:BaseUrl", "https://api.circle.com"},
                // Add Encryption key
                {"Encryption:Key", Convert.ToBase64String(new byte[32])},
                // Disable background services
                {"BackgroundServices:Enabled", "false"}
            }!);
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove ALL DbContext-related services to prevent provider conflicts
            var dbContextDescriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(AppDbContext)).ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing with unique name per test run
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}");
            });

            // Replace Circle API service with mock
            var circleServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICircleService));
            if (circleServiceDescriptor != null)
            {
                services.Remove(circleServiceDescriptor);
            }
            services.AddScoped<ICircleService, MockCircleService>();

            // Remove background services
            var hostedServices = services.Where(d => d.ServiceType.Name.Contains("IHostedService")).ToList();
            foreach (var service in hostedServices)
            {
                services.Remove(service);
            }
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Build the host - database seeding will happen automatically via AddDbContext
        var host = base.CreateHost(builder);
        return host;
    }
}
