using ClosedXML.Excel;
using apiB2e.Entities;

namespace apiB2e.Services;

public class ExcelExportService
{
    public byte[] ExportProductsToExcel(IEnumerable<Product> products)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Produtos");
        
        AddHeaders(worksheet);
        AddData(worksheet, products);
        FormatWorksheet(worksheet);
        
        return SaveToBytes(workbook);
    }

    private void AddHeaders(IXLWorksheet worksheet)
    {
        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Nome";
        worksheet.Cell(1, 3).Value = "Valor (R$)";
        worksheet.Cell(1, 4).Value = "Data de Inclusão";
    }

    private void AddData(IXLWorksheet worksheet, IEnumerable<Product> products)
    {
        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = product.IdProduto;
            worksheet.Cell(row, 2).Value = product.Nome;
            worksheet.Cell(row, 3).Value = product.Valor;
            worksheet.Cell(row, 4).Value = product.DataInclusao.ToString("dd/MM/yyyy HH:mm:ss");
            row++;
        }
    }

    private void FormatWorksheet(IXLWorksheet worksheet)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();
    }

    private byte[] SaveToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}