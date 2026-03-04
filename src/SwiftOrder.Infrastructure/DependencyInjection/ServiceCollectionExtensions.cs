using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Infrastructure.Persistence;
using SwiftOrder.Infrastructure.Persistence.Repositories;

namespace SwiftOrder.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IOrderItemWriter, OrderItemWriter>();

        return services;
    }
}