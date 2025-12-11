using Application.Interfaces;
using Domain.Model.POCO;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.ProcessAndStoreSapData
{
    public record ProcessAndStoreSapDataCommand(
        string InputPath,
        bool SaveToDatabase = true,
        bool GenerateExcel = true,
        string OutputPath = "") : IRequest<ProcessAndStoreResult>;

    public record ProcessAndStoreResult(
        bool Success,
        string Message,
        int RecordsProcessed,
        int RecordsSaved,
        int RecordsWithErrors,
        string OutputPath,
        DateTime ProcessedAt);

    public class ProcessAndStoreSapDataCommandHandler
        : IRequestHandler<ProcessAndStoreSapDataCommand, ProcessAndStoreResult>
    {
        private readonly IDataFileParser _parser;
        private readonly IExcelGenerator<BalanceData> _excelGenerator;
        private readonly IDataProcessor<BalanceData> _dataProcessor;
        private readonly ILogger<ProcessAndStoreSapDataCommandHandler> _logger;

        public ProcessAndStoreSapDataCommandHandler(
            IDataFileParser parser,
            IExcelGenerator<BalanceData> excelGenerator,
            IDataProcessor<BalanceData> dataProcessor,
            ILogger<ProcessAndStoreSapDataCommandHandler> logger)
        {
            _parser = parser;
            _excelGenerator = excelGenerator;
            _dataProcessor = dataProcessor;
            _logger = logger;
        }

        public async Task<ProcessAndStoreResult> Handle(
            ProcessAndStoreSapDataCommand command,
            CancellationToken cancellationToken)
        {
            int recordsProcessed = 0;
            int recordsSaved = 0;
            int recordsWithErrors = 0;

            try
            {
                // 1. Parsear datos
                _logger.LogInformation("Parseando archivo: {InputPath}", command.InputPath);
                var balanceData = await _parser.GetParsedBalanceDataAsync(
                    command.InputPath, cancellationToken);

                var dataList = balanceData.ToList();
                recordsProcessed = dataList.Count;

                // 2. Procesar y validar datos
                _logger.LogInformation("Procesando {Count} registros", recordsProcessed);
                var processingResult = await _dataProcessor.ProcessAndSaveAsync(
            dataList,  // ← Este método SÍ existe
            cancellationToken);

                // 3. Guardar en base de datos (si está habilitado)
                //if (command.SaveToDatabase)
                //{
                //    _logger.LogInformation("Guardando datos en base de datos");
                //    var validData = processingResult.ValidData;
                //    recordsSaved = validData.Count();
                //    recordsWithErrors = processingResult.Errors.Count();
                //}

                //// 4. Generar Excel (si está habilitado)
                //if (command.GenerateExcel && !string.IsNullOrEmpty(command.OutputPath))
                //{
                //    _logger.LogInformation("Generando archivo Excel: {OutputPath}", command.OutputPath);
                //    await _excelGenerator.GenerateExcelAsync(
                //        processedData,
                //        command.OutputPath,
                //        "Balance Procesado",
                //        cancellationToken);
                //}

                // 5. Calcular errores
                recordsWithErrors = recordsProcessed - recordsSaved;

                return new ProcessAndStoreResult(
                    Success: true,
                    Message: "Procesamiento completado exitosamente",
                    RecordsProcessed: recordsProcessed,
                    RecordsSaved: recordsSaved,
                    RecordsWithErrors: recordsWithErrors,
                    OutputPath: command.OutputPath,
                    ProcessedAt: DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando y almacenando datos SAP");
                return new ProcessAndStoreResult(
                    Success: false,
                    Message: $"Error: {ex.Message}",
                    RecordsProcessed: recordsProcessed,
                    RecordsSaved: recordsSaved,
                    RecordsWithErrors: recordsWithErrors,
                    OutputPath: command.OutputPath,
                    ProcessedAt: DateTime.UtcNow);
            }
        }
    }
}
