using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Exceptions
{
    public class ValidationException : ApplicationException
    {
        /// <summary>
        /// Diccionario que almacena los errores de validación de forma estructurada.
        /// Clave: Nombre de la propiedad que falló (ej. "Nombre").
        /// Valor: Array de mensajes de error asociados a esa propiedad.
        /// La propiedad solo tiene 'get', asegurando la inmutabilidad de los errores reportados.
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Constructor por defecto. Llama al constructor base con un mensaje genérico.
        /// Inicializa el diccionario de errores como vacío.
        /// </summary>
        public ValidationException() : base("Se presentaron uno o mas errores de validation")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Constructor principal. Recibe una colección de fallos de validación (de FluentValidation)
        /// y los transforma en la estructura de diccionario [Propiedad, Mensajes[]].
        /// </summary>
        /// <param name="failures">Colección de objetos ValidationFailure generados por FluentValidation.</param>
        public ValidationException(IEnumerable<ValidationFailure> failures) : this() // Llama al constructor por defecto.
        {
            // Utiliza LINQ para agrupar los fallos de validación por el nombre de la propiedad.
            Errors = failures
                        // Agrupa por el nombre de la propiedad y selecciona el mensaje de error.
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        // Convierte el resultado agrupado en un diccionario.
                        // La clave es el nombre de la propiedad (Property Name).
                        // El valor es un array de todos los mensajes de error para esa propiedad.
                        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
