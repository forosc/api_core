using Domain.Abstractions;
using Domain.Model.POCO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace Application.UseCases.ProcessSapData
{
    // Define el comando que se ejecutará
    public record ProcessSapDataCommand(string InputPath, string OutputPath);

    public class ProcessSapDataCommandHandler
    {
        private readonly IDataFileParser _parser;
        private readonly IExcelDataWriter _writer;

        public ProcessSapDataCommandHandler(IDataFileParser parser, IExcelDataWriter writer)
        {
            _parser = parser;
            _writer = writer;
        }

        public void Handle(ProcessSapDataCommand command)
        {
            // 1. Obtener los datos usando el Parser (lectura)
            IEnumerable<BalanceData> balanceData = _parser.GetParsedBalanceData(command.InputPath);

            // 2. Orquestación del ciclo de vida del archivo (detalles de la infraestructura)
            using (var spreadsheetDocument = SpreadsheetDocument.Create(command.OutputPath, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Balance" });

                using (var writer = OpenXmlWriter.Create(worksheetPart))
                {
                    writer.WriteStartElement(new Worksheet());
                    writer.WriteStartElement(new SheetData());

                    // Escribir cabeceras si es necesario...
                    // _writer.WriteRow(writer, new string[] { "Material", "Stock", "Moneda", ... });

                    // 3. Escribir cada fila usando el Writer (escritura)
                    foreach (var data in balanceData)
                    {
                        string[] rowValues = { data.Material, data.StockTotal, data.UnidadMedida, data.ValorTotal, data.Moneda };
                        _writer.WriteRow(writer, rowValues);
                    }

                    // 4. Cerrar elementos de la hoja
                    writer.WriteEndElement(); // SheetData
                    writer.WriteEndElement(); // Worksheet
                }
            }
        }
    }
}
