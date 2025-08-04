using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportService.Domain.Repositories;
using ReportService.Infrastructure.Repositories;
using Shared.Infrastructure;

namespace ReportService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReportServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ReportDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Add Repositories
            services.AddScoped<IReportRepository, ReportRepository>();

            // Add RabbitMQ
            services.AddRabbitMQ(configuration);

            return services;
        }
    }
}