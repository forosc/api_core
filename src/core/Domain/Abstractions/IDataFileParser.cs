using Domain.Model.POCO;

namespace Domain.Abstractions
{
    public interface IDataFileParser
    {
        IEnumerable<BalanceData> GetParsedBalanceData(string inputPath);
        IEnumerable<string[]> GetParsedMultiLineData(string inputPath);

        // Métodos de Detección (Requeridos por la Capa de Aplicación para decidir el flujo)
        string[]? DetectSapHeaders(string inputPath);
        bool IsDoubleLineFormat(string inputPath);
    }
}
