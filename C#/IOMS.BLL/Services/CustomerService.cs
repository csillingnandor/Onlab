using FluentValidation;
using IOMS.BLL.Exceptions;
using IOMS.BLL.Validation;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly IValidator<CreateCustomerData> _createValidator;

    public CustomerService(ICustomerRepository customers, IValidator<CreateCustomerData> createValidator)
    {
        _customers = customers;
        _createValidator = createValidator;
    }

    public Task<IReadOnlyList<CustomerData>> GetAllAsync(CancellationToken ct = default)
    {
        return _customers.GetAllAsync(ct);
    }

    public Task<CustomerData?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _customers.GetByIdAsync(id, ct);
    }

    public async Task<CustomerData> CreateAsync(CreateCustomerData data, CancellationToken ct = default)
    {
        // Felesleges szóközök nélkül mentünk; az üres telefonszám null.
        var normalized = new CreateCustomerData
        {
            Name = data.Name.Trim(),
            Email = data.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(data.Phone) ? null : data.Phone.Trim(),
        };

        // Előbb a formai szabályok, hogy hibás kérés ne menjen az adatbázisig
        await _createValidator.EnsureValidAsync(normalized, ct);

        // Az e-mail egyedi index; előre ellenőrizzük, hogy 500 helyett érthető hibát adjunk.
        if (await _customers.EmailExistsAsync(normalized.Email, ct))
            throw new BusinessValidationException(nameof(data.Email), $"Már létezik vevő ezzel az e-mail címmel: {normalized.Email}");

        return await _customers.CreateAsync(normalized, ct);
    }
}
