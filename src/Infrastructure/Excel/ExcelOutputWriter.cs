using Application.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace Infrastructure.Excel
{
    public class ExcelOutputWriter : IOutputWriter
    {
        private readonly OpenXmlWriter _writer;
        private readonly SpreadsheetDocument _document;

        public ExcelOutputWriter(string filePath)
        {
            _document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = _document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            _writer = OpenXmlWriter.Create(worksheetPart, Encoding.UTF8);

            _writer.WriteStartElement(new Worksheet());
            _writer.WriteStartElement(new SheetData());
        }

        public void WriteRow(string[] values)
        {
            WriteRowOutPutExcel.WriteRowToOutput(_writer, values);
        }

        public void Close()
        {
            _writer.WriteEndElement(); // SheetData
            _writer.WriteEndElement(); // Worksheet
            _writer.Close();
            //_document.Close();
        }
    }
}

