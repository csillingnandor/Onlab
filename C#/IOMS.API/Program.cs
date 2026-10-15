using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using IOMS.API.Infrastructure;
using IOMS.API.Validation;
using IOMS.BLL;
using IOMS.BLL.Auth;
using IOMS.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        // Enumok szövegként a JSON-ben ("Pending", nem 0), oda-vissza
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();

builder.Services
    .AddDal(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddBll(builder.Configuration["AutoMapper:LicenseKey"]);

// A kontrollerek által futtatott validátorok regisztrálása, magyar alapüzenetekkel
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDataValidator>();
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("hu");

// Bejelentkezés: az AuthService által kiadott JWT az Authorization: Bearer fejlécben
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
if (jwtOptions.Key.Length < 32)
    throw new InvalidOperationException("A Jwt:Key beállítás hiányzik vagy rövidebb 32 karakternél.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // A claim-ek a tokenbeli nevükön maradnak (sub, name, role, page), lásd AppClaimTypes
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            NameClaimType = AppClaimTypes.UserName,
            RoleClaimType = AppClaimTypes.Role,
        };
    });

// Oldalankénti jogosultság: [RequirePage(...)] a kontrollereken; ami nincs megjelölve, ahhoz is be kell jelentkezni
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Induláskor kiderül, ha egy Data mezőnek nincs leképezése (különben csendben üres maradna)
    app.Services.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

    app.MapOpenApi().AllowAnonymous();
}

// Alap szerepkörök és a konfigurált (fejlesztéshez: appsettings.Development.json) felhasználók
await app.Services.SeedIdentityAsync(builder.Configuration.GetSection("Seed:Users").Get<SeedUserOptions[]>() ?? []);

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
