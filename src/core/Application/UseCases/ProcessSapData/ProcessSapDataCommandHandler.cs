using Application.Interfaces;
using Domain.Model.POCO;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.ProcessSapData
{

    public class ProcessSapDataCommandHandler
        : IRequestHandler<ProcessSapDataCommand, ProcessSapDataResult>
    {
        private readonly IDataFileParser _parser;
        private readonly IExcelGenerator<BalanceData> _balanceExcelGenerator;
        private readonly IExcelGenerator<string[]> _multiLineExcelGenerator;
        private readonly ILogger<ProcessSapDataCommandHandler> _logger;

        public ProcessSapDataCommandHandler(
            IDataFileParser parser,
            IExcelGenerator<BalanceData> balanceExcelGenerator,
            IExcelGenerator<string[]> multiLineExcelGenerator,
            ILogger<ProcessSapDataCommandHandler> logger)
        {
            _parser = parser;
            _balanceExcelGenerator = balanceExcelGenerator;
            _multiLineExcelGenerator = multiLineExcelGenerator;
            _logger = logger;
        }

        public async Task<ProcessSapDataResult> Handle(
            ProcessSapDataCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Procesando archivo SAP: {InputPath} -> {OutputPath}",
                command.InputPath, command.OutputPath);

            int recordsProcessed = 0;
            string reportType = string.Empty;
            DateTime startTime = DateTime.UtcNow;

            try
            {
                // 1. Detectar formato
                bool isDoubleLine = _parser.IsDoubleLineFormat(command.InputPath);

                if (isDoubleLine)
                {
                    // Formato doble línea
                    var multiLineData = _parser.GetParsedMultiLineData(command.InputPath);
                    var dataList = multiLineData.ToList();
                    recordsProcessed = dataList.Count;
                    reportType = "Consumo/Rotación";

                    _logger.LogInformation(
                        "Formato doble línea detectado. {Count} registros encontrados",
                        recordsProcessed);

                    await _multiLineExcelGenerator.GenerateExcelAsync(
                        dataList,
                        command.OutputPath,
                        "Reporte de Consumo",
                        cancellationToken);
                }
                else
                {
                    // Formato simple (Balance)
                    var balanceData = await _parser.GetParsedBalanceDataAsync(
                        command.InputPath,
                        cancellationToken);

                    var dataList = balanceData.ToList();
                    recordsProcessed = dataList.Count;
                    reportType = "Balance";

                    _logger.LogInformation(
                        "Formato balance detectado. {Count} registros encontrados",
                        recordsProcessed);

                    await _balanceExcelGenerator.GenerateExcelAsync(
                        dataList,
                        command.OutputPath,
                        "Reporte de Balance",
                        cancellationToken);
                }

                var processingTime = DateTime.UtcNow - startTime;

                _logger.LogInformation(
                    "Procesamiento completado exitosamente. " +
                    "Tipo: {ReportType}, Registros: {Records}, Tiempo: {Time}ms",
                    reportType, recordsProcessed, processingTime.TotalMilliseconds);

                return new ProcessSapDataResult(
                    Success: true,
                    Message: $"Reporte {reportType} generado exitosamente",
                    RecordsProcessed: recordsProcessed,
                    OutputPath: command.OutputPath,
                    ProcessedAt: DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error procesando archivo SAP: {InputPath}",
                    command.InputPath);

                // Nota: El UnhandledExceptionBehavior capturará esta excepción
                // y la envolverá. Si quieres manejo específico aquí, puedes:
                throw new ApplicationException(
                    $"Error al procesar archivo SAP: {ex.Message}",
                    ex);
            }
        }
    }
}