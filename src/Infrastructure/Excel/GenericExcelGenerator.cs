using Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Excel
{
    public class GenericExcelGenerator<T> : IExcelGenerator<T>
    {
        private readonly IExcelDataWriter _excelDataWriter;

        public GenericExcelGenerator(IExcelDataWriter excelDataWriter)
        {
            _excelDataWriter = excelDataWriter;
        }

        public async Task GenerateExcelAsync(
            IEnumerable<T> data,
            string outputPath,
            string sheetName = "Data",  
            CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                using var spreadsheetDocument = SpreadsheetDocument.Create(
                    outputPath,
                    SpreadsheetDocumentType.Workbook);

                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = sheetName
                });

                using var writer = OpenXmlWriter.Create(worksheetPart);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());

                // Escribir encabezados automáticamente
                var properties = GetPropertiesForType<T>();
                var headers = GetHeaders(properties);
                _excelDataWriter.WriteRow(writer, headers);

                // Escribir datos
                foreach (var item in data)
                {
                    var rowValues = GetRowValues(item, properties);
                    _excelDataWriter.WriteRow(writer, rowValues);
                }

                writer.WriteEndElement(); // SheetData
                writer.WriteEndElement(); // Worksheet
            }, cancellationToken);
        }

        private PropertyInfo[] GetPropertiesForType<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private string[] GetHeaders(PropertyInfo[] properties)
        {
            return properties.Select(p => p.Name).ToArray();
        }

        private string[] GetRowValues(T item, PropertyInfo[] properties)
        {
            var values = new List<string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(item);
                values.Add(value?.ToString() ?? string.Empty);
            }

            return values.ToArray();
        }
    }
}
