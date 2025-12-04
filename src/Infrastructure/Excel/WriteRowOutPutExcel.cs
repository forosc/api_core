using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Infrastructure.Excel
{
    public static class WriteRowOutPutExcel
    {
        public static void WriteRowToOutput(OpenXmlWriter writer, string[] values)
        {
            writer.WriteStartElement(new Row());

            foreach (var val in values)
            {
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(val ?? "")
                };

                writer.WriteElement(cell);
            }

            writer.WriteEndElement(); // Row
        }
    }
}
