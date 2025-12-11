using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExcelGenerator<T>
    {
        Task GenerateExcelAsync(
            IEnumerable<T> data,
            string outputPath,
            string sheetName = "Data",
            CancellationToken cancellationToken = default);
    }
}
