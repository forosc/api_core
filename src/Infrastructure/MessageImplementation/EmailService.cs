using Application.Contracts.Infrastructure;
using Application.Models.Email;
using FluentEmail.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MessageImplementation
{
    
    public class EmailService : IEmailServices
    {
        private readonly IFluentEmail fluentEmail;
        private readonly EmailFluentSettings emailFluentSettings;
        // Asumimos ILogger también está inyectado para robustez (omitiendo el código por simplicidad)

        /// <summary>
        /// Constructor: Inyecta el cliente de correo configurado (FluentEmail) y las opciones de configuración.
        /// </summary>
        /// <param name="fluentEmail">Interfaz de FluentEmail para construir y enviar el correo.</param>
        /// <param name="emailFluentSettings">Interfaz IOptions para acceder a la configuración del correo (BaseUrl, FromEmail).</param>
        public EmailService(IFluentEmail fluentEmail, IOptions<EmailFluentSettings> emailFluentSettings)
        {
            this.fluentEmail = fluentEmail;
            // Se extrae el objeto de configuración (Value) para acceso directo.
            this.emailFluentSettings = emailFluentSettings.Value;
        }

        /// <summary>
        /// Envía un correo electrónico específico, diseñado para contener un enlace con un token (ej. Resetear Contraseña).
        /// </summary>
        /// <param name="email">Objeto que contiene los datos básicos del correo (To, Subject, Body).</param>
        /// <param name="token">Token de seguridad o confirmación.</param>
        /// <returns>True si el envío fue exitoso, false si falló.</returns>
        public async Task<bool>
            SendEmailAsync(EmailMessage email, string token)
        {
            // 1. Construcción Segura de la URI de acción
            // Combina la URL base del cliente (BaseUrlClient) con el path específico de reseteo y el token.
            // Uri.EscapeDataString() asegura que el token se codifique correctamente para ser usado en una URL.
            var resetUrl = new Uri(new Uri(emailFluentSettings.BaseUrlClient!), $"/password/reset/{Uri.EscapeDataString(token)}");

            // 2. Preparación del Contenido HTML
            // Agrega el enlace clickeable al cuerpo del correo. Se asume que email.Body puede contener HTML/texto previo.
            var htmlContent = $"{email.Body} <a href=\"{resetUrl}\">{resetUrl}</a>";

            // 3. Envío del Correo usando FluentEmail Builder
            var result = await fluentEmail.SetFrom(emailFluentSettings.Email) // Remitente por defecto
                .To(email.To)                                                 // Destinatario(s)
                .Subject(email.Subject)                                       // Asunto
                .Body(htmlContent, true)                                      // Cuerpo (Nota: agregar 'true' si htmlContent es HTML)
                .SendAsync();

            // 4. Manejo del Resultado (Implementación de Mejora: Loggear Errores)
            // En un entorno real, si result.Successful es false, se debería loggear el contenido de result.ErrorMessages
            // usando ILogger para diagnóstico.

            return result.Successful;
        }
    }
}
