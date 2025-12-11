using Application.Interfaces;
using Application.Models.Processed;
using Application.Persistence;
using Domain.Model.POCO;
using Infrastructure.Repositiries;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Processors
{
    public class DataProcessor : IDataProcessor<BalanceData>
    {
        private readonly ILogger<DataProcessor> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DataProcessor(ILogger<DataProcessor> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(int processed, int saved, int errors)> ProcessAndSaveAsync(
            IEnumerable<BalanceData> data,
            CancellationToken cancellationToken = default)
        {
            int totalProcessed = 0;
            int totalSaved = 0;
            int totalErrors = 0;

            // 1. VALIDACIÓN
            var validatedData = await ValidateDataAsync(data, cancellationToken);
            totalProcessed = data.Count();
            totalErrors = totalProcessed - validatedData.Count();

            // 2. Mapeo y Persistencia
            try
            {
                var repository = _unitOfWork.Repository<ProcessedBalanceData>();

                var entitiesToSave = validatedData.Select(d =>
                {
                    // Lógica de mapeo de POCO a Entidad de persistencia
                    return new ProcessedBalanceData
                    {
                        Material = d.Material,
                        StockTotal = decimal.Parse(d.StockTotal, CultureInfo.InvariantCulture), // Asumo CultureInfo.InvariantCulture
                        // ... (Mapeo de otras propiedades) ...
                        Status = ProcessingStatus.Processed,
                        ProcessingDate = DateTime.UtcNow
                    };
                }).ToList();

                await repository.AddRangeAsync(entitiesToSave);
                await _unitOfWork.CompleteAsync(cancellationToken);
                totalSaved = entitiesToSave.Count;

                _logger.LogInformation("Procesamiento completado. Guardados: {Saved}, Errores: {Errors}", totalSaved, totalErrors);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante la persistencia de datos.");
                // Si la persistencia falla, todos los registros validados no se guardan.
                totalSaved = 0;
                totalErrors = totalProcessed; // O un manejo más fino de errores.
            }

            return (totalProcessed, totalSaved, totalErrors);
        }

        public async Task<IEnumerable<BalanceData>> ValidateDataAsync(
            IEnumerable<BalanceData> data,
            CancellationToken cancellationToken = default)
        {
            var result = new List<BalanceData>();

            foreach (var item in data)
            {
                try
                {
                    // Validaciones básicas
                    if (string.IsNullOrEmpty(item.Material))
                    {
                        _logger.LogWarning("Registro sin Material: {Item}", item);
                        continue;
                    }

                    if (!decimal.TryParse(item.StockTotal, out _))
                    {
                        _logger.LogWarning("StockTotal inválido: {Item}", item);
                        continue;
                    }

                    // Puedes agregar más validaciones aquí
                    result.Add(item);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validando registro: {Item}", item);
                }
            }

            return await Task.FromResult(result);
        }
    }
}
