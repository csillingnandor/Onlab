using System.Globalization;
using FluentValidation;
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
        services.AddScoped<ISupplierOrderService, SupplierOrderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();

        // Az összes AbstractValidator<T> regisztrálása ebből a projektből, magyar alapüzenetekkel
        services.AddValidatorsFromAssemblyContaining<ProductService>(includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("hu");

        return services;
    }
}
