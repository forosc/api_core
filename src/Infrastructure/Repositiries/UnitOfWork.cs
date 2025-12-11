using Application.Persistence;
using Infrastructure.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositiries
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable? _repositories;
        private readonly DataDBContext _context;


        public UnitOfWork(DataDBContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Error en transaccion", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Confirma todos los cambios realizados en el contexto de la base de datos.
        /// </summary>
        /// <returns>El número de objetos cuyo estado fue guardado.</returns>
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            // Este es el único lugar donde se llama a SaveChangesAsync
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories is null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositoryBase<>);
                var repositoryInstance = Activator
                    .CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IAsyncRepository<TEntity>)_repositories[type]!;
        }
    }
}
