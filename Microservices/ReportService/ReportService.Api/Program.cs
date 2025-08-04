using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Repositories;
using ReportService.Application.Validators;
using Shared.Infrastructure.Messaging;
using Shared.Kernel.Events;
using Shared.Kernel.Behaviors;
using MediatR;
using Serilog;
using AspNetCoreRateLimit;
using Polly;
using Polly.CircuitBreaker;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ReportService.Application;
using ReportService.Infrastructure;
using Polly.Extensions.Http;
using ReportService.Domain;
using ReportService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/reportservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-API-Version")
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Circuit Breaker
builder.Services.AddHttpClient("ReportService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
})
.AddPolicyHandler(GetCircuitBreakerPolicy());

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Database
builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateReportCommand).Assembly);
});

// Validation
builder.Services.AddValidatorsFromAssembly(typeof(CreateReportDtoValidator).Assembly);

// Cross-Cutting Concerns - Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

// Caching (Optional - only for queries that implement ICachableQuery)
builder.Services.AddMemoryCache();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

try
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
        dbContext.Database.Migrate();
        dbContext.Database.OpenConnection(); // Try to open the connection
        dbContext.Database.CloseConnection(); // Close the connection
        Log.Information("Database connection successful.");
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Database connection failed: {Message}", ex.Message);
}

// RabbitMQ Consumer
builder.Services.AddSingleton<IEventConsumer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var mediator = provider.GetRequiredService<IMediator>();

    return new RabbitMqConsumer(configuration, async (@event) =>
    {
        try
        {
            Log.Information("Processing report request: {ReportId}", @event.ReportId);

            // Process the report request
            var createReportDto = new CreateReportDto
            {
                ReportId = @event.ReportId,
                RequestedAt = @event.RequestedAt
            };

            var createResult = await mediator.Send(new CreateReportCommand(createReportDto, "System"));
            if (!createResult.IsSuccess)
            {
                Log.Error("Failed to create report: {Error}", createResult.Error);
                return;
            }

            // TODO: Add logic to fetch contact data and generate statistics
            // For now, just update the status to completed
            var updateResult = await mediator.Send(new UpdateReportStatusCommand(@event.ReportId, ReportStatus.Completed, "Sample statistics", "System"));
            if (!updateResult.IsSuccess)
            {
                Log.Error("Failed to update report status: {Error}", updateResult.Error);
            }
            else
            {
                Log.Information("Report processed successfully: {ReportId}", @event.ReportId);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "RabbitMQ consumer error: {Message}", ex.Message);
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Rate Limiting Middleware
app.UseIpRateLimiting();

// Global Exception Handler
app.UseExceptionHandler("/error");

app.UseHttpsRedirection();
app.UseAuthorization();

// Health Check Endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Start RabbitMQ Consumer
var consumer = app.Services.GetRequiredService<IEventConsumer>();
await consumer.StartConsumingAsync();

app.Run();

// Circuit Breaker Policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );
}
