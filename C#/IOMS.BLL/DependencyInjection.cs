using IOMS.BLL.Auth;
using IOMS.BLL.Mapping;
using IOMS.BLL.Services;
using IOMS.DAL;
using IOMS.DAL.Entities;
using Microsoft.AspNetCore.Identity;
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

        // Felhasználók és szerepkörök (Identity, EF tárolással); a bejelentkezés JWT-t ad (AuthService),
        // ezért elég az AddIdentityCore, cookie-alapú SignInManager nem kell.
        services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddErrorDescriber<HungarianIdentityErrorDescriber>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
