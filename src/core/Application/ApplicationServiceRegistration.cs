using Application.Behavior;
using Application.Extentions;
using Application.Mappings;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            var applicationAssembly = Assembly.GetExecutingAssembly();

            // 1. MediatR
            services.AddMediatR(applicationAssembly);

            // 2. FluentValidation
            services.AddValidatorsFromAssembly(applicationAssembly);

            // 3. AutoMapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            services.AddSingleton(mapperConfig.CreateMapper());

            // 4. MediatR Behaviors (orden importante)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

            // 5. Servicios de Application
            services.AddServiceEmail(configuration);

            return services;
        }
    }
}
