using Application.Behavior;
using Application.Extentions;
using Application.Mappings;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationServiceRegistration
    {
        /// <summary>
        /// Método de extensión para registrar todos los servicios y configuraciones 
        /// pertenecientes a la capa de Aplicación en el contenedor de IoC de .NET.
        /// </summary>
        /// <param name="services">Colección de servicios de .NET Core (IServiceCollection).</param>
        /// <param name="configuration">Objeto de configuración de la aplicación (IConfiguration).</param>
        /// <returns>La misma colección de servicios con las nuevas dependencias añadidas.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. REGISTRO DE AUTOMAPPER

            // Define la configuración para AutoMapper, buscando un perfil de mapeo.
            var mapperConfig = new MapperConfiguration(mc =>
            {
                // Busca y añade un perfil de mapeo (MappingProfile) donde se definen las transformaciones entre DTOs y Entidades.
                mc.AddProfile(new MappingProfile());
            });

            // Crea una instancia del objeto Mapper basado en la configuración.
            IMapper mapper = mapperConfig.CreateMapper();

            // Registra el IMapper como un servicio Singleton (una única instancia compartida por toda la aplicación).
            services.AddSingleton(mapper);

            // 2. REGISTRO DE COMPORTAMIENTOS DE MEDIATR (Pipeline Behaviors)
            // Se asume que la aplicación utiliza MediatR (o una librería similar de CQRS) 
            // y estos son los "middlewares" que se ejecutan antes/después de un comando o consulta.

            // Registra el comportamiento para manejar excepciones no controladas. 
            // Esto envuelve la ejecución del Request/Command en un bloque try-catch centralizado.
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

            // Registra el comportamiento para realizar la validación (ej. con FluentValidation).
            // Esto valida el Request/Command antes de que llegue al Handler principal.
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // 3. REGISTRO DE SERVICIOS ESPECÍFICOS

            // Llama a un método de extensión externo (probablemente en una capa de Infraestructura/Compartida) 
            // para registrar las dependencias relacionadas con el envío de correos electrónicos.
            // Se le pasa el IConfiguration porque la configuración del email (ej. SMTP) suele estar en appsettings.json.
            services.AddServiceEmail(configuration);

            // Devuelve la colección de servicios actualizada para permitir el encadenamiento de métodos.
            return services;
        }
    }
}
