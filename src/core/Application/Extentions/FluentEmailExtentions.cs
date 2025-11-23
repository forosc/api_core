using Application.Models.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extentions
{
    public static class FluentEmailExtentions
    {
        /// <summary>
        /// Método de extensión estático para configurar y registrar el servicio de envío de correos electrónicos 
        /// utilizando la librería FluentEmail y el transporte SMTP, obteniendo los ajustes de IConfiguration.
        /// </summary>
        /// <param name="services">La colección de servicios de .NET Core (contenedor IoC).</param>
        /// <param name="configuration">La fuente de configuración de la aplicación (ej. appsettings.json).</param>
        public static void AddServiceEmail(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. REGISTRO DE OPCIONES (Options Pattern)

            // Enlaza la sección 'EmailFluentSettings' del archivo de configuración 
            // a la clase DTO 'EmailFluentSettings'. Esto permite que otras clases 
            // inyecten IOptions<EmailFluentSettings> para acceder a los valores de forma fuertemente tipada.
            services.Configure<EmailFluentSettings>(configuration.GetSection(nameof(EmailFluentSettings)));

            // 2. LECTURA DIRECTA PARA EL REGISTRO DE FLUENTEMAIL

            // Obtiene la sección de configuración. Se lee directamente porque FluentEmail 
            // necesita estos parámetros inmediatos para configurar el SmtpSender.
            var emailSettings = configuration.GetSection(nameof(EmailFluentSettings));

            // Lee los valores específicos del servidor SMTP y el remitente.
            var fromEmail = emailSettings.GetValue<string>("Email");
            var host = emailSettings.GetValue<string>("Host");
            var port = emailSettings.GetValue<int>("Port");

            // 3. REGISTRO DEL SERVICIO DE CORREO (FluentEmail)

            // Configura el remitente por defecto para FluentEmail y registra la infraestructura base.
            var fluentEmailBuilder = services.AddFluentEmail(fromEmail);

            // Registra el SmtpSender como el mecanismo de transporte de correos, 
            // usando el host y puerto leídos de la configuración.
            fluentEmailBuilder.AddSmtpSender(host, port);

            // Nota: Para autenticación (usuario/contraseña), se añadirían parámetros adicionales
            // a AddSmtpSender, también leídos de la configuración.
        }
    }
}
