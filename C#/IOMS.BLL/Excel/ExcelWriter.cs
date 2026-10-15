using ClosedXML.Excel;

namespace IOMS.BLL.Excel;

// Egy oszlop: fejléc, érték a sorból és (opcionálisan) Excel számformátum
public record ExcelColumn<T>(string Header, Func<T, object?> Value, string? Format = null);

internal static class ExcelWriter
{
    // Munkalap félkövér, rögzített fejléccel, szűrővel és a tartalomhoz igazított oszlopszélességgel
    public static void AddSheet<T>(XLWorkbook workbook, string name, IReadOnlyList<ExcelColumn<T>> columns, IEnumerable<T> rows)
    {
        var sheet = workbook.Worksheets.Add(name);
        for (var c = 0; c < columns.Count; c++)
            sheet.Cell(1, c + 1).Value = columns[c].Header;

        var r = 2;
        foreach (var row in rows)
        {
            for (var c = 0; c < columns.Count; c++)
            {
                var cell = sheet.Cell(r, c + 1);
                cell.Value = ToCellValue(columns[c].Value(row));
                if (columns[c].Format is { } format)
                    cell.Style.NumberFormat.Format = format;
            }
            r++;
        }

        var header = sheet.Range(1, 1, 1, columns.Count);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#DCE6F1");
        sheet.SheetView.FreezeRows(1);
        if (r > 2)
            sheet.Range(1, 1, r - 1, columns.Count).SetAutoFilter();
        sheet.Columns().AdjustToContents(1, Math.Min(r, 500));
    }

    public static byte[] ToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static XLCellValue ToCellValue(object? value) => value switch
    {
        null => Blank.Value,
        string s => s,
        int i => i,
        long l => l,
        decimal d => d,
        double d => d,
        bool b => b,
        DateTime dt => dt.Kind == DateTimeKind.Utc ? dt.ToLocalTime() : dt,
        _ => value.ToString(),
    };
}
