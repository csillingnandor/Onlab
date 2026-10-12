using IOMS.BLL.Mapping;
using IOMS.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IOMS.BLL;

public static class DependencyInjection
{
    // licenseKey: az AutoMapper (15+) licenckulcsa; nélküle is működik, csak induláskor figyelmeztetést naplóz
    public static IServiceCollection AddBll(this IServiceCollection services, string? autoMapperLicenseKey)
    {
        // Entitás -> Data leképezések (ProjectTo): Mapping/MappingProfile.cs
        services.AddAutoMapper(cfg => cfg.LicenseKey = autoMapperLicenseKey, typeof(MappingProfile));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerOrderService, CustomerOrderService>();
        services.AddScoped<ICustomerOrderItemService, CustomerOrderItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<ISupplierOrderService, SupplierOrderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();

        return services;
    }
}
