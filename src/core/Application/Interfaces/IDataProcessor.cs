using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDataProcessor<T>
    {
        Task<(int processed, int saved, int errors)> ProcessAndSaveAsync(
            IEnumerable<T> data,
            CancellationToken cancellationToken = default);
    }
 
}
