using System.Globalization;
using ClosedXML.Excel;
using IOMS.BLL.Exceptions;
using IOMS.DAL;
using IOMS.DAL.Entities;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Excel;

// Termékek tömeges felvétele / frissítése Excelből, két lépésben: előnézet (hibák a mentés előtt), majd mentés.
// A mentéskor a fájlt újra feldolgozzuk (nem a kliens által visszaküldött sorokat mentjük), és csak hibátlan fájl kerülhet be.
// A soronkénti formai ellenőrzést (validateRow) az API adja a CreateProductData validátorával, hogy a szabályok
// ugyanazok legyenek, mint az egyenkénti felvételnél.
public interface IProductImportService
{
    byte[] CreateTemplate();

    Task<ProductImportPreviewData> PreviewAsync(
        Stream file, Func<CreateProductData, IDictionary<string, string[]>> validateRow, CancellationToken ct = default);

    // BusinessValidationException, ha a fájl hibás (a hibák az előnézetben látszanak)
    Task<ProductImportResultData> ImportAsync(
        Stream file, Func<CreateProductData, IDictionary<string, string[]>> validateRow, CancellationToken ct = default);
}

internal class ProductImportService : IProductImportService
{
    public const int MaxRows = 5000;

    // Oszlopfejlécek (kis-nagybetű és szóközök nem számítanak); az első néhány kötelező
    private const string NameHeader = "Név";
    private const string SkuHeader = "SKU";
    private const string CategoryHeader = "Kategória";
    private const string PriceHeader = "Ár";
    private const string MinStockHeader = "Minimális készlet";

    private static readonly string[] RequiredHeaders = [NameHeader, SkuHeader, PriceHeader];
    private static readonly string[] KnownHeaders = [NameHeader, SkuHeader, CategoryHeader, PriceHeader, MinStockHeader];

    private readonly AppDbContext _context;

    public ProductImportService(AppDbContext context)
    {
        _context = context;
    }

    public byte[] CreateTemplate()
    {
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Termékek", [
            new ExcelColumn<CreateProductData>(NameHeader, p => p.Name),
            new(SkuHeader, p => p.SKU),
            new(CategoryHeader, p => p.Category),
            new(PriceHeader, p => p.Price),
            new(MinStockHeader, p => p.MinStockLevel),
        ], [new CreateProductData { Name = "Minta termék", SKU = "MINTA-001", Category = "Perifériák", Price = 9990m, MinStockLevel = 5 }]);
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<ProductImportPreviewData> PreviewAsync(
        Stream file, Func<CreateProductData, IDictionary<string, string[]>> validateRow, CancellationToken ct = default)
    {
        var preview = new ProductImportPreviewData();

        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(file);
        }
        catch (Exception)
        {
            preview.SchemaErrors.Add("A fájl nem olvasható Excel (.xlsx) munkafüzetként.");
            return preview;
        }

        using (workbook)
        {
            var sheet = workbook.Worksheets.FirstOrDefault();
            var headerRow = sheet?.FirstRowUsed();
            if (sheet is null || headerRow is null)
            {
                preview.SchemaErrors.Add("A munkafüzet üres.");
                return preview;
            }

            // Fejléc -> oszlopszám
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.CellsUsed())
            {
                var header = cell.GetString().Trim();
                var known = KnownHeaders.FirstOrDefault(h => string.Equals(h, header, StringComparison.OrdinalIgnoreCase));
                if (known is null)
                    preview.Warnings.Add($"Ismeretlen oszlop, figyelmen kívül marad: „{header}”");
                else if (!columns.TryAdd(known, cell.Address.ColumnNumber))
                    preview.SchemaErrors.Add($"A(z) „{known}” oszlop többször szerepel.");
            }

            foreach (var required in RequiredHeaders.Where(h => !columns.ContainsKey(h)))
                preview.SchemaErrors.Add($"Hiányzik a kötelező „{required}” oszlop (az első sorban a fejlécnek kell lennie).");
            if (preview.SchemaErrors.Count > 0)
                return preview;

            var dataRows = sheet.RowsUsed().Where(r => r.RowNumber() > headerRow.RowNumber()).ToList();
            if (dataRows.Count > MaxRows)
            {
                preview.SchemaErrors.Add($"Legfeljebb {MaxRows} sor importálható egyszerre (a fájlban {dataRows.Count} van).");
                return preview;
            }

            foreach (var row in dataRows)
                preview.Rows.Add(ParseRow(row, columns, validateRow));
        }

        if (preview.Rows.Count == 0)
            preview.SchemaErrors.Add("A fejlécen kívül nincs adat a munkalapon.");

        // A fájlon belüli SKU-ismétlés: mindegyik előfordulás hibás, mert nem dönthető el, melyik a jó
        foreach (var group in preview.Rows.Where(r => !string.IsNullOrWhiteSpace(r.Sku))
                     .GroupBy(r => r.Sku!, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
        {
            var rowNumbers = string.Join(", ", group.Select(r => r.RowNumber));
            foreach (var row in group)
                AddError(row, nameof(row.Sku), $"Az SKU többször szerepel a fájlban (sorok: {rowNumbers}).");
        }

        // Létező SKU: frissítés, egyébként új termék
        var skus = preview.Rows.Where(r => !string.IsNullOrWhiteSpace(r.Sku)).Select(r => r.Sku!).Distinct().ToList();
        var existing = await _context.Products
            .Where(p => skus.Contains(p.SKU))
            .Select(p => p.SKU)
            .ToListAsync(ct);
        var existingSet = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
        foreach (var row in preview.Rows)
            row.Action = row.Sku is not null && existingSet.Contains(row.Sku) ? "Update" : "Create";

        preview.ErrorCount = preview.Rows.Count(r => r.Errors.Count > 0);
        preview.CreateCount = preview.Rows.Count(r => r.Errors.Count == 0 && r.Action == "Create");
        preview.UpdateCount = preview.Rows.Count(r => r.Errors.Count == 0 && r.Action == "Update");
        return preview;
    }

    public async Task<ProductImportResultData> ImportAsync(
        Stream file, Func<CreateProductData, IDictionary<string, string[]>> validateRow, CancellationToken ct = default)
    {
        var preview = await PreviewAsync(file, validateRow, ct);
        if (preview.SchemaErrors.Count > 0)
            throw new BusinessValidationException(string.Empty, string.Join(" ", preview.SchemaErrors));
        if (preview.ErrorCount > 0)
            throw new BusinessValidationException(string.Empty,
                $"A fájlban {preview.ErrorCount} hibás sor van; javítsd őket, és töltsd fel újra.");

        var skus = preview.Rows.Select(r => r.Sku!).ToList();
        var existing = await _context.Products
            .Where(p => skus.Contains(p.SKU))
            .ToDictionaryAsync(p => p.SKU, StringComparer.OrdinalIgnoreCase, ct);

        var result = new ProductImportResultData();
        foreach (var row in preview.Rows)
        {
            if (!existing.TryGetValue(row.Sku!, out var product))
            {
                product = new Product { SKU = row.Sku! };
                _context.Products.Add(product);
                result.Created++;
            }
            else
            {
                result.Updated++;
            }

            product.Name = row.Name!;
            product.Category = row.Category ?? string.Empty;
            product.Price = row.Price!.Value;
            // Ha a fájlban nincs minimum oszlop / üres, a meglévő érték marad (újnál 0)
            if (row.MinStockLevel is { } min)
                product.MinStockLevel = min;
        }

        // Egy SaveChanges egy tranzakció: vagy minden sor bekerül, vagy egyik sem
        await _context.SaveChangesAsync(ct);
        return result;
    }

    private static ProductImportRowData ParseRow(IXLRow row, Dictionary<string, int> columns,
        Func<CreateProductData, IDictionary<string, string[]>> validateRow)
    {
        var result = new ProductImportRowData { RowNumber = row.RowNumber() };

        string? Text(string header) =>
            columns.TryGetValue(header, out var col) && row.Cell(col).GetString().Trim() is { Length: > 0 } text ? text : null;

        result.Name = Text(NameHeader);
        result.Sku = Text(SkuHeader);
        result.Category = Text(CategoryHeader);

        if (columns.TryGetValue(PriceHeader, out var priceCol))
        {
            var (price, error) = ReadDecimal(row.Cell(priceCol));
            result.Price = price;
            if (error) AddError(result, nameof(result.Price), $"Az ár nem szám: „{row.Cell(priceCol).GetString()}”");
        }
        if (columns.TryGetValue(MinStockHeader, out var minCol))
        {
            var (min, error) = ReadDecimal(row.Cell(minCol));
            if (error || (min is { } m && m != Math.Floor(m)))
                AddError(result, nameof(result.MinStockLevel), $"A minimális készlet nem egész szám: „{row.Cell(minCol).GetString()}”");
            else
                result.MinStockLevel = min is null ? null : (int)min.Value;
        }

        if (result.Price is null && !result.Errors.ContainsKey(nameof(result.Price)))
            AddError(result, nameof(result.Price), "Az ár megadása kötelező.");

        // Ugyanazok a formai szabályok, mint az egyenkénti felvételnél
        var data = new CreateProductData
        {
            Name = result.Name ?? string.Empty,
            SKU = result.Sku ?? string.Empty,
            Category = result.Category,
            Price = result.Price ?? 0,
            MinStockLevel = result.MinStockLevel ?? 0,
        };
        foreach (var (field, messages) in validateRow(data))
        {
            var key = field switch
            {
                nameof(CreateProductData.SKU) => nameof(result.Sku),
                _ => field,
            };
            // Az ár hiányát / formai hibáját már jeleztük, ne legyen dupla üzenet
            if (key == nameof(result.Price) && result.Errors.ContainsKey(key))
                continue;
            foreach (var message in messages)
                AddError(result, key, message);
        }

        return result;
    }

    // Számcella vagy szövegként beírt szám (magyar "9 990,5" és angol "9990.5" alak is); üres cella: null
    private static (decimal? Value, bool Error) ReadDecimal(IXLCell cell)
    {
        if (cell.IsEmpty())
            return (null, false);
        if (cell.DataType == XLDataType.Number)
            return ((decimal)cell.GetDouble(), false);

        var text = cell.GetString().Trim().Replace(" ", "").Replace(" ", "").Replace("Ft", "", StringComparison.OrdinalIgnoreCase);
        if (text.Length == 0)
            return (null, false);
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.GetCultureInfo("hu-HU"), out var hu))
            return (hu, false);
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var inv))
            return (inv, false);
        return (null, true);
    }

    private static void AddError(ProductImportRowData row, string field, string message)
    {
        row.Errors[field] = row.Errors.TryGetValue(field, out var existing) ? [.. existing, message] : [message];
    }
}
