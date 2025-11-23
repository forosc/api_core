using Application.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IAsyncRepository<T> where T : class
    {
        // --- LECTURA BÁSICA (Asíncrona y de Solo Lectura) ---

        /// <summary>
        /// Recupera todas las entidades de forma asíncrona (debe usar AsNoTracking en la implementación).
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Recupera una entidad por su clave primaria (ID).
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Recupera una lista de entidades filtradas por un predicado WHERE simple.
        /// </summary>
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

        // --- ESCRITURA (Unit of Work - Sin SaveChangesAsync) ---
        // Estas operaciones modifican el estado de EF Core pero NO guardan en la DB. 
        // La persistencia debe ser gestionada por una Unidad de Trabajo o un Servicio.

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        void AddRange(IEnumerable<T> entities); // Usa IEnumerable<T> para más flexibilidad
        void DeleteRange(IEnumerable<T> entities); // Usa IEnumerable<T>

        // --- PATRÓN SPECIFICATION (Consultas Complejas) ---

        /// <summary>
        /// Recupera la primera entidad que coincide con los criterios, inclusiones y ordenación de la Specification.
        /// </summary>
        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);

        /// <summary>
        /// Recupera una lista de entidades aplicando todos los criterios de la Specification.
        /// </summary>
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        /// <summary>
        /// Cuenta el número de entidades que cumplen con los criterios de la Specification.
        /// Es esencial para la paginación.
        /// </summary>
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
