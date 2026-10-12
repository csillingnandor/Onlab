using System.Globalization;
using System.Text.Json.Serialization;
using FluentValidation;
using IOMS.API.Infrastructure;
using IOMS.API.Validation;
using IOMS.BLL;
using IOMS.DAL;

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

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
