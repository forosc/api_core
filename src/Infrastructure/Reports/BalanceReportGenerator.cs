using Application.Interfaces.Reports;
using Application.Interfaces;
using Domain.Model.POCO;

namespace Infrastructure.Reports
{
    public class BalanceReportGenerator : IBalanceReportGenerator
    {
        private readonly IExcelGenerator<BalanceData> _excelGenerator;

        public BalanceReportGenerator(IExcelGenerator<BalanceData> excelGenerator)
        {
            _excelGenerator = excelGenerator;
        }

        public Task GenerateReportAsync(
            IEnumerable<BalanceData> balanceData,
            string outputPath,
            CancellationToken cancellationToken = default)
        {
            return _excelGenerator.GenerateExcelAsync(
                balanceData,
                outputPath,
                "Balance",
                cancellationToken);
        }
    }
}
