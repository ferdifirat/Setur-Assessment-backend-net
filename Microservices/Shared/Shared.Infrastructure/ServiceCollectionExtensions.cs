using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Messaging;

namespace Shared.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add RabbitMQ
            services.AddRabbitMQ(configuration);

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSection = configuration.GetSection("RabbitMQ");

            services.Configure<RabbitMQSettings>(rabbitMQSection);

            services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
            services.AddScoped<IEventPublisher, RabbitMqPublisher>();
            services.AddScoped<IEventConsumer, RabbitMqConsumer>();

            return services;
        }
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string User { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}