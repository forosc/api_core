using Domain.Abstractions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Globalization;
using System.Text.RegularExpressions;
using System;

namespace Infrastructure.Writers
{
    public class ExcelDataWriter : IExcelDataWriter
    {
        // El record ahora pertenece a esta capa de Infraestructura
        public record OutputFile(OpenXmlWriter Writer, SpreadsheetDocument Document, string FilePath);

        public void WriteRow(OpenXmlWriter writer, string[] values)
        {
            if (writer == null) return;

            writer.WriteStartElement(new Row());

            foreach (var value in values)
            {
                string cellValue = value ?? "";
                var cell = new Cell();

                try
                {
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        // Estas validaciones específicas de formato de celda son detalles de infraestructura
                        if (Regex.IsMatch(cellValue, @"^\d{1,2}([./])\d{1,2}\1\d{2,4}$"))
                        {
                            string normalizedDate = NormalizeDates(cellValue);
                            cell.DataType = CellValues.String;
                            cellValue = normalizedDate;
                        }
                        else if (Regex.IsMatch(cellValue, @"^-?[\d,]+([.,]\d+)?-?$"))
                        {
                            string normalizedNumber = NormalizeNumber(cellValue);
                            if (decimal.TryParse(normalizedNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                            {
                                cell.DataType = CellValues.Number;
                                cellValue = normalizedNumber;
                            }
                            else
                            {
                                cell.DataType = CellValues.String;
                            }
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                        }
                    }
                    else
                    {
                        cell.DataType = CellValues.String;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando valor en celda: {ex.Message}");
                    cell.DataType = CellValues.String;
                }

                cell.CellValue = new CellValue(cellValue);
                writer.WriteElement(cell);
            }

            writer.WriteEndElement();
        }

        // --------------------------------------------------------
        // 🛠️ MÉTODOS AUXILIARES (PRIVADOS - duplicados simplificados, ya que la lógica original los usaba)
        // --------------------------------------------------------

        // Nota: En un sistema real, moverías la lógica de NormalizeNumber y NormalizeDates a un DTO Mapper
        // o un Validation Service en esta capa, no duplicándola. Aquí está para que el código compile.

        private static string NormalizeDates(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            input = input.Trim().Replace('.', '/');
            if (DateTime.TryParse(input, out DateTime dt))
            {
                return dt.ToString("d/M/yyyy");
            }
            return input;
        }

        // Versión simplificada para la escritura
        private static string NormalizeNumber(string input)
        {
            // Lógica de normalización idéntica a la del parser
            if (string.IsNullOrWhiteSpace(input)) return "";
            input = input.Trim();
            if (input.EndsWith("-")) { input = "-" + input.Substring(0, input.Length - 1); }
            input = input.Replace(" ", "");
            if (Regex.IsMatch(input, @"^\d{1,3}(\.\d{3})+,\d+$")) { input = input.Replace(".", "").Replace(",", "."); }
            else if (Regex.IsMatch(input, @"^\d{1,3}(,\d{3})+\.\d+$")) { input = input.Replace(",", ""); }
            else if (input.Contains(",") && !input.Contains(".") && input.Count(c => c == ',') == 1) { input = input.Replace(",", "."); }
            return input;
        }
    }
}