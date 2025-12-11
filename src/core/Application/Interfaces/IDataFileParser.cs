using Domain.Model.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDataFileParser
    {
        Task<IEnumerable<BalanceData>> GetParsedBalanceDataAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        IEnumerable<string[]> GetParsedMultiLineData(string inputPath);
        bool IsDoubleLineFormat(string inputPath);
        string[]? DetectSapHeaders(string inputPath);
    }
}
