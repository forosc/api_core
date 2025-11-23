using Application.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            // 1. APLICAR FILTRADO (WHERE)
            // Aplica el predicado de filtrado (Criteria) si está definido en la Specification.
            if (spec.Criteria != null)
            {
                inputQuery = inputQuery.Where(spec.Criteria);
            }

            // 2. APLICAR ORDENACIÓN (ORDER BY / ORDER BY DESCENDING)
            // NOTA CLAVE: La ordenación debe aplicarse antes de la paginación para garantizar 
            // que los resultados sean determinísticos (siempre los mismos).
            // 
            // Uso 'else if' para asegurar que solo se aplique UNA ordenación primaria,
            // corrigiendo el potencial conflicto del código original donde OrderByDescending
            // siempre sobreescribiría a OrderBy si ambos estuvieran definidos.
            if (spec.OrderByDescending != null)
            {
                inputQuery = inputQuery.OrderByDescending(spec.OrderByDescending);
            }
            else if (spec.OrderBy != null)
            {
                inputQuery = inputQuery.OrderBy(spec.OrderBy);
            }
            // Para ordenaciones secundarias (ThenBy), la Specification debería exponer una colección
            // de expresiones y la lógica de ThenBy debería aplicarse aquí después del OrderBy primario.

            // 3. APLICAR PAGINACIÓN (SKIP Y TAKE)
            // Se aplica solo si la paginación está habilitada.
            if (spec.IsPagingEnable)
            {
                // Se asume que Skip y Take están definidos correctamente si IsPagingEnable es true.
                inputQuery = inputQuery.Skip(spec.Skip).Take(spec.Take);
            }

            // 4. APLICAR INCLUDES, SPLIT QUERY Y NO TRACKING

            // Aplica todas las propiedades de navegación para "eager loading" (Includes).
            // Utilizamos 'Aggregate' para encadenar múltiples .Include().
            // Se añade una verificación explícita de null para mayor seguridad.
            if (spec.Includes != null)
            {
                inputQuery = spec.Includes
                    .Aggregate(inputQuery, (current, include) => current.Include(include));
            }

            // Configura la consulta final con optimizaciones de EF Core:
            // .AsSplitQuery(): Evita el "cartesian explosion" en consultas con múltiples Includes,
            //                  ejecutando múltiples consultas SQL por separado (EF Core >= 5.0).
            // .AsNoTracking(): Optimización para consultas de solo lectura, deshabilitando el 
            //                  seguimiento de cambios en el DbContext, lo que mejora el rendimiento.
            inputQuery = inputQuery.AsSplitQuery().AsNoTracking();

            return inputQuery;
        }
    }

}
