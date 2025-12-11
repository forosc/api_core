
namespace Application.UseCases.ProcessSapData
{
    public record ProcessSapDataResult(
        bool Success,
        string Message,
        int RecordsProcessed,
        string OutputPath,
        DateTime ProcessedAt);
}
