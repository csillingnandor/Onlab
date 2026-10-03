using IOMS.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IOMS.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerOrderService, CustomerOrderService>();
        services.AddScoped<ICustomerOrderItemService, CustomerOrderItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        return services;
    }
}
