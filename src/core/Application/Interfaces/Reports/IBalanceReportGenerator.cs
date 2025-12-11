using Domain.Model.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Reports
{
    public interface IBalanceReportGenerator
    {
        Task GenerateReportAsync(
            IEnumerable<BalanceData> balanceData,
            string outputPath,
            CancellationToken cancellationToken = default);
    }
}
