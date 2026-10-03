using IOMS.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IOMS.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDal(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerOrderRepository, CustomerOrderRepository>();
        services.AddScoped<ICustomerOrderItemRepository, CustomerOrderItemRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        return services;
    }
}
