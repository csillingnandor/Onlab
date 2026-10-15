using FluentValidation;
using IOMS.API.Infrastructure;
using IOMS.BLL.Auth;
using IOMS.BLL.Excel;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

// Termékimport Excelből: sablon letöltése, előnézet (soronkénti hibákkal), majd mentés.
// Az előnézet és a mentés is a feltöltött fájlt dolgozza fel (multipart/form-data, "file" mező).
[ApiController]
[Route("api/import")]
[RequireAction(AppActions.ProductsImport)]
public class ImportController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IProductImportService _importService;
    private readonly IValidator<CreateProductData> _productValidator;

    public ImportController(IProductImportService importService, IValidator<CreateProductData> productValidator)
    {
        _importService = importService;
        _productValidator = productValidator;
    }

    [HttpGet("products/template")]
    public IActionResult Template() => File(_importService.CreateTemplate(), XlsxContentType, "termek-import-sablon.xlsx");

    [HttpPost("products/preview")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ProductImportPreviewData>> Preview(IFormFile file, CancellationToken ct)
    {
        if (CheckFile(file) is { } error)
            return error;

        await using var stream = file.OpenReadStream();
        return Ok(await _importService.PreviewAsync(stream, ValidateRow, ct));
    }

    // 400, ha a fájl hibás (ugyanazok a hibák, mint az előnézetben)
    [HttpPost("products")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ProductImportResultData>> Import(IFormFile file, CancellationToken ct)
    {
        if (CheckFile(file) is { } error)
            return error;

        await using var stream = file.OpenReadStream();
        return Ok(await _importService.ImportAsync(stream, ValidateRow, ct));
    }

    private ActionResult? CheckFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            ModelState.AddModelError("file", "Nincs feltöltött fájl.");
        else if (file.Length > MaxFileSize)
            ModelState.AddModelError("file", "A fájl legfeljebb 5 MB lehet.");
        else if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError("file", "Csak .xlsx fájl tölthető fel.");
        return ModelState.IsValid ? null : ValidationProblem(ModelState);
    }

    // Ugyanaz a validátor, mint az egyenkénti termékfelvételnél
    private IDictionary<string, string[]> ValidateRow(CreateProductData data) =>
        _productValidator.Validate(data).Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}
