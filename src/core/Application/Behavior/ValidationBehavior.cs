using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> // Restricción: Asegura que TRequest es un comando o consulta de MediatR.
    {
        /// <summary>
        /// Colección de validadores (FluentValidation) registrados en el contenedor de IoC para el tipo TRequest.
        /// Si existe un validador (o varios) para un Request específico, se inyectarán aquí.
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Constructor: Recibe la lista de validadores inyectados automáticamente por el contenedor de dependencias.
        /// </summary>
        /// <param name="validators">Lista de IValidator<TRequest> encontrados.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Método Handle: Intercepta la solicitud (Request) y ejecuta la lógica de validación.
        /// </summary>
        /// <param name="request">El comando o consulta actual que se está procesando.</param>
        /// <param name="next">Delegado que representa el siguiente paso en el pipeline (usualmente el Request Handler).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>La respuesta TResponse.</returns>
        public async Task<TResponse> Handle(
                                TRequest request,
                                RequestHandlerDelegate<TResponse> next,
                                CancellationToken cancellationToken
                                )
        {
            // 1. Verificar la existencia de validadores
            if (_validators.Any())
            {
                // 2. Ejecutar la validación

                // Crea el contexto de validación requerido por FluentValidation.
                var context = new ValidationContext<TRequest>(request);

                // Ejecuta asíncronamente todos los validadores registrados para esta solicitud en paralelo.
                var validationResults = await Task
                    .WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // 3. Consolidar los errores

                // Aplana y filtra los resultados para obtener una lista única de errores (failures).
                var failures = validationResults
                    .SelectMany(r => r.Errors) // Selecciona todos los errores de todos los validadores.
                    .Where(f => f != null)     // Asegura que no haya errores nulos.
                    .ToList();

                // 4. Lanzar excepción si hay fallos
                if (failures.Count != 0)
                {
                    // Si se encontraron errores, se lanza una ValidationException.
                    // Esta excepción detiene inmediatamente la ejecución del Request y permite 
                    // que el código de la API (o capa superior) lo maneje, devolviendo usualmente un código 400 Bad Request.
                    throw new ValidationException(failures);
                }
            }

            // 5. Continuar con el siguiente paso del Pipeline
            // Si no hay validadores o la validación fue exitosa, se llama al delegado 'next()',
            // permitiendo que el Request Handler final (la lógica de negocio) se ejecute.
            return await next();
        }
    }
}
