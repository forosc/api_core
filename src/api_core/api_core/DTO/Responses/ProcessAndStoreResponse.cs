namespace api_core.DTO.Responses
{
    public record ProcessAndStoreResponse(
        bool Success,
        string Message,
        int RecordsProcessed,
        int RecordsSaved,
        int RecordsWithErrors,
        string OutputPath,
        DateTime ProcessedAt);
}
