using Domain.Model.POCO;
using System.Text;
using System.Text.RegularExpressions;
using Infrastructure.Configuration;
using Application.Interfaces;

namespace Infrastructure.Parsers
{
    public class SapDataFileParser : IDataFileParser
    {
        private record ConsumptionDataWithUnits(string Consumption, string ConsumptionUnit, string Stock, string StockUnit);

        public IEnumerable<BalanceData> GetParsedBalanceData(string inputPath)
        {
            using var sr = new StreamReader(inputPath, Encoding.GetEncoding(1252));
            string? line;
            string currentCuentaMayor = "";

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line) || IsBalanceFileProblematicLine(line)) continue;

                if (Regex.IsMatch(line, @"^\d{9,}"))
                {
                    var cuentaMatch = Regex.Match(line, @"^(\d{9,})");
                    if (cuentaMatch.Success) currentCuentaMayor = cuentaMatch.Groups[1].Value;
                    continue;
                }

                var balanceData = ParseSimpleMaterialLine(line);
                if (balanceData != null)
                {
                    balanceData.CuentaMayor = currentCuentaMayor;
                    yield return balanceData;
                }
            }
        }

        public async Task<IEnumerable<BalanceData>> GetParsedBalanceDataAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Archivo no encontrado: {filePath}");

            var balanceDataList = new List<BalanceData>();

            try
            {
                // Leer todas las líneas del archivo
                var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

                // Saltar encabezados si es necesario (depende del formato SAP)
                int startIndex = DetermineStartIndex(lines);

                for (int i = startIndex; i < lines.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var balanceData = ParseLine(line);
                    if (balanceData != null)
                    {
                        balanceDataList.Add(balanceData);
                    }
                }

                return balanceDataList;
            }
            catch (Exception ex)
            {
                // Loggear error según tu estrategia de logging
                throw new InvalidOperationException(
                    $"Error procesando archivo SAP: {ex.Message}", ex);
            }
        }

        private int DetermineStartIndex(string[] lines)
        {
            // Lógica para detectar encabezados del archivo SAP
            // Esto depende del formato específico de tu archivo

            // Ejemplo: Saltar las primeras 3 líneas si son encabezados
            if (lines.Length > 0 && lines[0].Contains("Material"))
                return 1;

            // Otra lógica común: buscar línea que contenga datos numéricos
            for (int i = 0; i < Math.Min(lines.Length, 10); i++)
            {
                if (IsDataLine(lines[i]))
                    return i;
            }

            return 0; // Empezar desde el principio si no se detectan encabezados
        }

        private bool IsDataLine(string line)
        {
            // Verificar si la línea contiene datos (no encabezados/comentarios)
            return !string.IsNullOrWhiteSpace(line) &&
                   !line.StartsWith("#") &&
                   !line.StartsWith("//") &&
                   !line.StartsWith("Material") &&
                   !line.StartsWith("***");
        }

        private BalanceData ParseLine(string line)
        {
            try
            {
                // Dependiendo del formato del archivo SAP (CSV, TXT, etc.)
                // Ejemplo para CSV separado por punto y coma (formato SAP común)
                var columns = line.Split(';', StringSplitOptions.TrimEntries);

                // Asegúrate de que tienes suficientes columnas
                if (columns.Length < 5) // Ajusta según tu estructura
                    return null;

                return new BalanceData
                {
                    Material = NormalizeString(columns[0]),
                    StockTotal = NormalizeNumber(columns[1]),
                    UnidadMedida = NormalizeString(columns[2]),
                    ValorTotal = NormalizeNumber(columns[3]),
                    Moneda = NormalizeString(columns[4])
                };
            }
            catch (Exception ex)
            {
                // Loggear error de parsing para esta línea específica
                Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                return null;
            }
        }

        private string NormalizeString(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Trim();
        }


        public IEnumerable<string[]> GetParsedMultiLineData(string inputPath)
        {
            using var sr = new StreamReader(inputPath, Encoding.GetEncoding(1252));
            string? line;
            string[]? currentMaterial = null; // Almacena los datos de la Línea 1

            while ((line = sr.ReadLine()) != null)
            {
                string originalLine = line;
                line = line.Trim();

                // 1. Filtrar líneas de basura/encabezados (usando helpers encapsulados)
                if (string.IsNullOrWhiteSpace(line) || IsInvalidLine(line) || IsGenericHeaderLine(line))
                    continue;

                // 2. 🔹 LÍNEA 1: Detección de Material (inicia nuevo registro)
                // Patrón: Pipe '|' seguido inmediatamente por un dígito (el Material)
                if (originalLine.StartsWith("|") && originalLine.Length > 1 && char.IsDigit(originalLine[1]))
                {
                    string cleanLine = line.Trim('|').Trim();

                    var parts = Regex.Split(cleanLine, @"\s{2,}")
                                       .Select(p => p.Trim())
                                       .Where(p => !string.IsNullOrWhiteSpace(p))
                                       .ToArray();

                    if (parts.Length >= 2)
                    {
                        // Pre-asignar las 7 columnas: Material, Texto, Grado
                        currentMaterial = new string[7];

                        currentMaterial[0] = parts[0];
                        currentMaterial[1] = parts.Length > 2
                                                ? string.Join(" ", parts.Skip(1).Take(parts.Length - 2))
                                                : (parts.Length > 1 ? parts[1] : "");

                        // Usamos el helper encapsulado para el Grado de Rotación
                        currentMaterial[2] = parts.Length >= 3 ? NormalizeNumber(parts[^1]) : "";
                    }
                }
                // 3. 🔹 LÍNEA 2: Detección de Consumo/Stock (une el registro)
                // Patrón: Pipe '|' seguido de un espacio en blanco (línea de continuación)
                else if (currentMaterial != null && originalLine.StartsWith("|") && originalLine.Length > 1 && char.IsWhiteSpace(originalLine[1]))
                {
                    string cleanLine = line.Trim('|').Trim();

                    // Usamos el método auxiliar para extraer Consumo y Stock
                    var consumptionData = ExtractConsumptionAndStockWithUnits(cleanLine);

                    if (consumptionData != null)
                    {
                        // Asignar datos de la segunda línea
                        currentMaterial[3] = consumptionData.Consumption;
                        currentMaterial[4] = consumptionData.ConsumptionUnit;
                        currentMaterial[5] = consumptionData.Stock;
                        currentMaterial[6] = consumptionData.StockUnit;

                        // Devolver el registro completo ensamblado
                        yield return currentMaterial;
                        currentMaterial = null; // Reiniciar el estado
                    }
                    else
                    {
                        currentMaterial = null; // Descartar si el consumo es inválido
                    }
                }
                // 4. Descartar la Línea 1 si la Línea 2 no sigue inmediatamente
                else if (currentMaterial != null)
                {
                    currentMaterial = null;
                }
            }
        }

       
        // Método auxiliar que maneja la lógica de dividir la segunda línea.
        private static ConsumptionDataWithUnits? ExtractConsumptionAndStockWithUnits(string line)
        {
            try
            {
                // 1. Dividir la línea de Consumo/Stock
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                 .Where(p => !string.IsNullOrWhiteSpace(p))
                                 .ToArray();

                // 2. Validar que tengamos Consumo, Unidad, Stock, Unidad
                if (parts.Length >= 4)
                {
                    return new ConsumptionDataWithUnits(
                        Consumption: NormalizeNumber(parts[0]),
                        ConsumptionUnit: parts[1],
                        Stock: NormalizeNumber(parts[2]),
                        StockUnit: parts[3]
                    );
                }

                // Manejar el caso de solo Consumo y Stock (sin unidades) como fallback si es necesario
                if (parts.Length >= 2)
                {
                    return new ConsumptionDataWithUnits(
                       Consumption: NormalizeNumber(parts[0]),
                       ConsumptionUnit: "",
                       Stock: NormalizeNumber(parts[1]),
                       StockUnit: ""
                   );
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static BalanceData? ParseSimpleMaterialLine(string line)
        {
            try
            {
                var match = Regex.Match(line, @"([A-Z0-9]{2,4})\s+(\d{6,})\s+([\d\.,]+)\s+([A-Z]{2,4})\s+([\d\.,]+)\s+([A-Z]{2,4})");
                if (match.Success)
                {
                    return new BalanceData
                    {
                        Almacen = match.Groups[1].Value,
                        Material = match.Groups[2].Value,
                        StockTotal = NormalizeNumber(match.Groups[3].Value),
                        UnidadMedida = match.Groups[4].Value,
                        ValorTotal = NormalizeNumber(match.Groups[5].Value),
                        Moneda = match.Groups[6].Value
                    };
                }
                return null;
            }
            catch { return null; }
        }

        private static bool IsBalanceFileProblematicLine(string line)
        {
            return string.IsNullOrWhiteSpace(line) || Regex.IsMatch(line, @"^[\-\=\.\s]{10,}$") || line.Contains("Total");
        }

        private static string NormalizeNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            string result = input.Replace(".", "").Replace(",", ".");
            return result;
        }

        /// <summary>
        /// Determina si el archivo tiene el formato de doble línea (consumo/rotación SAP).
        /// </summary>
        /// <param name="inputPath">Ruta del archivo a verificar.</param>
        /// <returns>True si se detecta el patrón de doble línea.</returns>
        public bool IsDoubleLineFormat(string inputPath)
        {
            try
            {
                // La lógica I/O para la detección se mantiene aquí.
                using var sr = new StreamReader(inputPath, Encoding.GetEncoding(1252));
                string? line;
                int pairCount = 0;
                bool expectingConsumption = false;

                while ((line = sr.ReadLine()) != null && pairCount < 5)
                {
                    string originalLine = line;
                    line = line.Trim();

                    if (string.IsNullOrWhiteSpace(line) || IsInvalidLine(line))
                        continue;

                    if (IsGenericHeaderLine(line))
                        continue;

                    // Lógica de detección de pares (material + consumo)
                    if (originalLine.StartsWith("|") && originalLine.Length > 1 && char.IsDigit(originalLine[1]))
                    {
                        expectingConsumption = true;
                    }
                    else if (expectingConsumption && originalLine.StartsWith("|") && originalLine.Length > 1 && char.IsWhiteSpace(originalLine[1]))
                    {
                        pairCount++;
                        expectingConsumption = false;
                    }
                    else
                    {
                        expectingConsumption = false;
                    }
                }
                return pairCount >= 2;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Intenta identificar un patrón de encabezado SAP en las primeras líneas.
        /// </summary>
        /// <param name="inputPath">Ruta del archivo.</param>
        /// <returns>El tipo de headers detectados o null.</returns>
        public string[]? DetectSapHeaders(string inputPath)
        {
            using var sr = new StreamReader(inputPath, Encoding.GetEncoding(1252));
            string? line;
            int lineCount = 0;

            while ((line = sr.ReadLine()) != null && lineCount < 100)
            {
                line = line.Trim().Trim('|').Trim();
                lineCount++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 1. Detección por patrones obligatorios (ej. Hora + Material + Cantidad)
                bool quickMatchFound = SapKeywords.HeaderDetectionPatterns.Any(requiredSet =>
                {
                    // Comprueba si TODOS los elementos del conjunto están presentes en la línea
                    return requiredSet.All(keyword =>
                        line.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                });

                // 2. Detección por conteo genérico de palabras clave SAP (fallback)
                bool internalMatchFound = IsSapHeaderLineInternal(line);

                if (quickMatchFound || internalMatchFound)
                {
                    // Si detectamos headers de algún tipo, procedemos a parsear la línea de headers
                    // (Esta línea divide la línea por delimitadores y devuelve el array de nombres de columna)
                    return ParseSapHeadersInternal(line);
                }

                // 3. Detección de headers de saldos SAP (que tienen un output fijo)
                // Utilizamos la lógica IsSapBalanceHeaderLineInternal que ya refactorizamos
                if (IsSapBalanceHeaderLineInternal(line))
                {
                    // Devolver los headers de salida fijos desde la configuración
                    return SapKeywords.StandardBalanceOutputHeaders;
                }
            }
            return null;
        }


        // ========================================================
        // 🛠️ MÉTODOS AUXILIARES INTERNOS (PRIVADOS)
        // ========================================================

        // El resto de los métodos se hacen privados para encapsular la lógica:

        private static bool IsSapHeaderLineInternal(string line)
        {
            int keywordMatches = SapKeywords.GenericSapHeaders.Count(keyword =>
                line.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            return keywordMatches >= 3;
        }

        private static string[] ParseSapHeadersInternal(string headerLine)
        {
            // Lógica original de ParseSapHeaders (división por pipes, tabs, espacios múltiples)
            if (headerLine.Contains('|')) return headerLine.Split('|').Select(p => p.Trim()).ToArray();
            if (headerLine.Contains('\t')) return headerLine.Split('\t').Select(p => p.Trim()).ToArray();
            return Regex.Split(headerLine, @"\s{2,}").Select(p => p.Trim()).ToArray();
        }

        private static bool IsSapBalanceHeaderLineInternal(string line)
        {
            // 1. Verificar los patrones obligatorios (ej. "Cta.mayor" DEBE estar con "Materiales Mon.")
            bool requiredPatternsFound = SapKeywords.RequiredBalancePatterns.Any(requiredSet =>
            {
                // Comprueba si TODOS los elementos del conjunto requerido están presentes en la línea
                return requiredSet.All(keyword =>
                    line.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            });

            if (requiredPatternsFound)
            {
                return true;
            }

            // 2. Fallback: Simplificación por conteo de keywords
            // Si no se encontró un patrón estricto, verifica si hay al menos 2 keywords relevantes de Balance

            int keywordMatches = SapKeywords.BalanceKeywords.Count(keyword =>
                line.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            // El umbral de 2 keywords es el más flexible y evita el código fijo.
            return keywordMatches >= 2;
        }

        private static bool IsInvalidLine(string line)
        {
            // Lógica original IsInvalidLine (separadores, asteriscos, etc.)
            return line.All(c => c == '-' || c == '=') || line.StartsWith("*");
        }

        private static bool IsGenericHeaderLine(string line)
        {
            string clean = line.Trim().Trim('|').Trim();

            // Usamos las palabras clave del nuevo directorio
            int keywordCount = SapKeywords.DoubleLineKeywords.Count(keyword =>
                clean.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            return keywordCount >= 2;
        }
    }
}