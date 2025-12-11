namespace api_core.DTO.Responses
{
    public record ProcessSapResponse(
        bool Success,
        string Message,
        int RecordsProcessed,
        string OutputPath,
        DateTime ProcessedAt);
}
