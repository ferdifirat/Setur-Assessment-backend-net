using ContactService.Domain;
using ContactService.Infrastructure.Data;
using ContactService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure;
using Shared.Infrastructure.Messaging;

namespace ContactService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContactServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ContactDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Add Repositories
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactInformationRepository, ContactInformationRepository>();

            // Add RabbitMQ
            services.AddRabbitMQ(configuration);

            return services;
        }
    }
} 