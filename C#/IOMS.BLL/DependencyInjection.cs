using IOMS.BLL.Auth;
using IOMS.BLL.Excel;
using IOMS.BLL.Mapping;
using IOMS.BLL.Notifications;
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
        services.AddScoped<IPickRouteService, PickRouteService>();
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

        // E-mail: a kérések sorba teszik a leveleket, a háttérszolgáltatás küldi őket (beállítások: EmailOptions)
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddHostedService<EmailBackgroundService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDailySummaryService, DailySummaryService>();
        services.AddHostedService<DailySummaryBackgroundService>();

        // Excel export és termékimport (ClosedXML)
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IProductImportService, ProductImportService>();

        return services;
    }
}
