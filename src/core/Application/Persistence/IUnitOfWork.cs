using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
        Task<int> Complete();
    }
}
