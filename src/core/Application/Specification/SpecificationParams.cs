using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Specification
{
    public abstract class SpecificationParams
    {
        // --- PROPIEDADES DE ORDENACIÓN Y BÚSQUEDA ---

        /// <summary>
        /// Cadena que especifica el criterio de ordenación deseado (ej. "priceAsc", "dateDesc").
        /// La lógica para interpretar y aplicar este valor debe residir en las clases Specification derivadas.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Cadena para búsquedas de texto libre o parciales (ej. LIKE %...%).
        /// </summary>
        public string? Search { get; set; }

        // --- PROPIEDADES DE PAGINACIÓN ---

        /// <summary>
        /// Índice de la página solicitada. Por defecto, se establece en la primera página (1).
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Constante que define el tamaño máximo permitido para la página (guardia de seguridad).
        /// Esto previene que un cliente solicite una cantidad excesiva de datos en una sola petición.
        /// </summary>
        private const int MaxPageSize = 50;

        /// <summary>
        /// Campo privado de respaldo para el tamaño de la página, inicializado con un valor por defecto.
        /// </summary>
        private int _pageSize = 3;

        /// <summary>
        /// Tamaño de la página solicitada. Utiliza un accesor 'set' personalizado para aplicar la restricción MaxPageSize.
        /// Si el valor solicitado es mayor que MaxPageSize, se usa MaxPageSize en su lugar.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
