using Application.Persistence;
using Application.Specification; // Contiene ISpecification<T>
using Infrastructure.Specification;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositiries
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : class
    {
        protected readonly DataDBContext _context;

        public RepositoryBase(DataDBContext context)
        {
            _context = context;
        }

        // --- LECTURA BÁSICA (AsNoTracking por defecto para rendimiento) ---

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // --- ESCRITURA (Unit of Work - Sin SaveChangesAsync) ---

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        // --- PATRÓN SPECIFICATION ---

        /// <summary>
        /// Método interno que evalúa la Specification para construir el IQueryable<T>.
        /// </summary>
        protected IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            // CORRECCIÓN: Se asume que SpecificationEvaluator<T> está en el namespace 
            // Infrastructure.Specification o fue importado correctamente.
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
    }
}