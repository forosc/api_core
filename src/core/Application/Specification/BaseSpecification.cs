using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Specification
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        // --- CONSTRUCTORES ---

        /// <summary>
        /// Constructor por defecto. Permite crear una especificación sin criterios de filtrado iniciales.
        /// </summary>
        public BaseSpecification()
        {
            // El cuerpo se deja vacío, ya que las colecciones se inicializan directamente en la declaración de la propiedad.
        }

        /// <summary>
        /// Constructor principal que inicializa la especificación con un criterio de filtrado (cláusula WHERE).
        /// </summary>
        /// <param name="criteria">La expresión lambda que define el filtro (ej. c => c.EstaActivo).</param>
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        // --- PROPIEDADES DE CONSULTA ---

        /// <summary>
        /// Almacena el predicado de filtrado para la cláusula WHERE. Es inmutable una vez establecido en el constructor.
        /// </summary>
        public Expression<Func<T, bool>>? Criteria { get; }

        /// <summary>
        /// Lista de expresiones de navegación para la carga ávida (eager loading) de entidades relacionadas (cláusulas Include).
        /// Se inicializa con una lista vacía para evitar excepciones NullReferenceException.
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// Almacena la expresión para la ordenación ascendente (OrderBy). Se establece a través del método protegido AddOrderBy.
        /// </summary>
        public Expression<Func<T, object>>? OrderBy { get; private set; }

        /// <summary>
        /// Almacena la expresión para la ordenación descendente (OrderByDescending). Se establece a través del método protegido AddOrderByDescending.
        /// </summary>
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        // --- PROPIEDADES DE PAGINACIÓN ---

        /// <summary>
        /// Número de registros a tomar (Take). Se establece a través del método protegido ApplyPaging.
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// Número de registros a omitir (Skip). Se establece a través del método protegido ApplyPaging.
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// Bandera que indica si la paginación debe ser aplicada a la consulta. Se establece a true en ApplyPaging.
        /// </summary>
        public bool IsPagingEnable { get; private set; }

        // --- MÉTODOS PROTEGIDOS (API DE CONSTRUCCIÓN) ---

        /// <summary>
        /// Agrega una expresión de ordenación ascendente (OrderBy).
        /// Se utiliza 'protected' para que solo las clases que hereden de BaseSpecification puedan llamar a este método.
        /// </summary>
        /// <param name="orderByExpression">Expresión lambda para la columna de ordenación.</param>
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        /// <summary>
        /// Agrega una expresión de ordenación descendente (OrderByDescending).
        /// </summary>
        /// <param name="orderByExpression">Expresión lambda para la columna de ordenación.</param>
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByExpression)
        {
            OrderByDescending = orderByExpression;
        }

        /// <summary>
        /// Habilita la paginación y establece los valores de Skip y Take.
        /// </summary>
        /// <param name="skip">Número de registros a omitir.</param>
        /// <param name="take">Número de registros a tomar.</param>
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnable = true; // El flag se activa automáticamente al aplicar la paginación.
        }

        /// <summary>
        /// Agrega una expresión de inclusión (Include) para cargar propiedades de navegación.
        /// Esto permite encadenar múltiples .Include() en la consulta final.
        /// </summary>
        /// <param name="includeExpression">Expresión lambda de la propiedad de navegación a incluir.</param>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
