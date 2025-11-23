using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Behavior
{
    // Esta clase implementa IPipelineBehavior de MediatR, lo cual permite interceptar y envolver la ejecución
    // de un request (comando o consulta) para agregar lógica adicional como manejo de errores, validación, etc.
    public class UnhandledExceptionBehavior<TRequest, TResponse>
                            : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        // Inyecta un ILogger específico para el tipo de request. Esto permite registrar logs con el contexto del request que falla.
        public UnhandledExceptionBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        // Este método es llamado por MediatR cuando se ejecuta cualquier request en el pipeline.
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                // Intenta ejecutar el siguiente paso en el pipeline (normalmente el request handler).
                return await next();
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción, captura el nombre del request para mayor claridad en el log.
                var requestName = typeof(TRequest).Name;

                // Registra el error con detalles del tipo de request y su contenido.
                _logger.LogError(ex,
                    "Application Request: Sucedió una excepción para el request {Name} {@Request}",
                    requestName, request);

                // Lanza una nueva excepción genérica. ⚠️ Aquí podrías perder detalles de la excepción original.
                // Es mejor relanzar la excepción original si no necesitas ocultarla.
                throw new Exception($"Application Request con Errores = {ex.Message}");
            }
        }
    }
}
