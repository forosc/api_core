using Application.Interfaces; // Para IOutputWriter
using Domain.Abstractions;    // Para IDataFileParser
using Domain.Services;        // Asumimos que DomainValueParser está aquí
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class FileProcessorService
    {
        private readonly IDataFileParser _parser;
        private readonly Func<string, IOutputWriter> _outputWriterFactory;

        /// <summary>
        /// Constructor que inyecta el Parser de Dominio y la Fábrica de Escritura.
        /// </summary>
        public FileProcessorService(IDataFileParser parser, Func<string, IOutputWriter> outputWriterFactory)
        {
            _parser = parser;
            _outputWriterFactory = outputWriterFactory;
        }

        /// <summary>
        /// Procesa un archivo, detecta su formato SAP y escribe los datos normalizados.
        /// </summary>
        /// <param name="inputPath">Ruta del archivo de entrada.</param>
        /// <param name="outputPath">Ruta donde se creará el archivo de salida.</param>
        public void Process(string inputPath, string outputPath)
        {
            IEnumerable<string[]> rowsToProcess;
            string[]? detectedHeaders = null;

            // 1. 🔍 LLAMADO A LOS MÉTODOS DE DETECCIÓN (Desde IDataFileParser)
            bool isDoubleLine = _parser.IsDoubleLineFormat(inputPath);

            if (!isDoubleLine)
            {
                // Solo detectamos headers si no es el formato de doble línea, 
                // ya que la detección de doble línea es más específica.
                detectedHeaders = _parser.DetectSapHeaders(inputPath);
            }

            // 2. 📖 LLAMADO AL MÉTODO DE PARSING CORRECTO
            if (isDoubleLine)
            {
                // Formato doble línea (Consumo/Rotación)
                rowsToProcess = _parser.GetParsedMultiLineData(inputPath);
            }
            else if (detectedHeaders != null)
            {
                // Formato simple (Saldos o Movimientos)
                var balanceData = _parser.GetParsedBalanceData(inputPath);

                // Mapeo final de POCO a string[] para la escritura (coordinación)
                rowsToProcess = balanceData.Select(d => new string[]
                {
                    d.CuentaMayor,
                    d.Almacen,
                    d.Material,
                    d.StockTotal,
                    d.UnidadMedida,
                    d.ValorTotal,
                    d.Moneda
                }).ToArray();
            }
            else
            {
                throw new InvalidOperationException($"No se pudo determinar el formato SAP del archivo: {inputPath}");
            }

            // 3. CREAR Y USAR EL WRITER (Fábrica de Infraestructura)
            IOutputWriter outputWriter = _outputWriterFactory(outputPath);

            try
            {
                // Opcional: Escribir headers si fueron detectados
                if (detectedHeaders != null)
                {
                    outputWriter.WriteRow(detectedHeaders);
                }

                foreach (var row in rowsToProcess)
                {
                    // Lógica de normalización de valores (Capa de Aplicación)
                    var normalized = row.Select(value =>
                    {
                        if (DomainValueParser.IsDate(value))
                            return DomainValueParser.NormalizeDate(value);

                        if (DomainValueParser.IsNumber(value))
                            return DomainValueParser.NormalizeNumber(value);

                        return value ?? "";
                    }).ToArray();

                    outputWriter.WriteRow(normalized);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando o escribiendo datos: {ex.Message}");
                throw;
            }
            finally
            {
                // 4. LLAMADO AL MÉTODO DE CIERRE (Infraestructura)
                outputWriter.Close();
            }
        }
    }
}