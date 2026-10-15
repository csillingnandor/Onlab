namespace IOMS.DTO;

// Termékimport előnézete: a feltöltött Excel feldolgozott sorai a hibáikkal, még mentés előtt
public class ProductImportPreviewData
{
    // A fájl egészét érintő hibák (nem Excel, hiányzó kötelező oszlop...); ha van, a sorok nem értelmezhetők
    public List<string> SchemaErrors { get; set; } = [];

    // Nem végzetes megjegyzések (pl. ismeretlen, figyelmen kívül hagyott oszlop)
    public List<string> Warnings { get; set; } = [];

    public List<ProductImportRowData> Rows { get; set; } = [];

    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int ErrorCount { get; set; }
}

public class ProductImportRowData
{
    // A sor száma az Excelben (a fejléc az 1.), hogy a hibát könnyű legyen megtalálni
    public int RowNumber { get; set; }

    public string? Name { get; set; }
    public string? Sku { get; set; }
    public string? Category { get; set; }
    public decimal? Price { get; set; }
    public int? MinStockLevel { get; set; }

    // "Create": új termék; "Update": már létező SKU, a termék adatai frissülnek
    public string Action { get; set; } = "Create";

    // Mezőnként a hibák (a mezőnév a fenti tulajdonság neve, "" a sor egészére vonatkozik)
    public Dictionary<string, string[]> Errors { get; set; } = [];
}

public class ProductImportResultData
{
    public int Created { get; set; }
    public int Updated { get; set; }
}
