using Application.Contracts.Identity;
using Application.Models.Token;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.Auth
{
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Propiedad que almacena la configuración de JWT (clave, tiempo de expiración, etc.), 
        /// obtenida del sistema de configuración de .NET (ej. appsettings.json) a través del Options Pattern.
        /// </summary>
        public JwtSettings _jwtSettings { get; }

        /// <summary>
        /// Interfaz para acceder a los datos del contexto HTTP actual (solicitud, sesión, usuario).
        /// Es esencial para obtener información del usuario autenticado en la sesión actual.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor: Inyecta las dependencias necesarias.
        /// </summary>
        /// <param name="httpContextAccessor">Accede al contexto de la solicitud actual.</param>
        /// <param name="jwtSettings">Accede a la configuración de JWT mediante IOptions<T>.</param>
        public AuthService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            // Extrae el objeto de configuración (Value) para acceso directo.
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Crea y firma un JWT (JSON Web Token) basado en la información del usuario y sus roles.
        /// </summary>
        /// <param name="usuario">Objeto que contiene los datos del usuario (ID, UserName, Email).</param>
        /// <param name="roles">Lista de roles asignados al usuario.</param>
        /// <returns>El JWT codificado como cadena.</returns>
        public string CreateToken(User usuario, IList<string>? roles)
        {
            // 1. DEFINICIÓN DE CLAIMS (Carga útil del Token)
            // Las Claims son pares clave-valor que contienen información sobre el usuario y las permisos.
            var claims = new List<Claim> {
            // Claim estándar para el nombre del sujeto.
            new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName!),
            // Claims personalizadas o estándar para información clave.
            new Claim("userId", usuario.Id),
            new Claim("email", usuario.Email!)
        };

            // 2. AGREGAR ROLES
            // Itera sobre la lista de roles y los agrega como claims de tipo 'Role'.
            foreach (var rol in roles!) // Se usa '!' asumiendo que roles no es null o se maneja en el caller.
            {
                var claim = new Claim(ClaimTypes.Role, rol);
                claims.Add(claim);
            }

            // 3. FIRMA Y CREDENCIALES

            // Define la clave secreta (SymmetricSecurityKey) a partir del string de configuración.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));

            // Crea las credenciales de firma utilizando la clave y el algoritmo HMAC SHA-512.
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. DESCRIPTOR DEL TOKEN
            // Objeto que contiene todos los metadatos necesarios para crear el token.
            var tokenDescription = new SecurityTokenDescriptor
            {
                // Asigna la identidad (ClaimsIdentity) con las claims definidas.
                Subject = new ClaimsIdentity(claims),
                // Define el tiempo de expiración basado en la configuración.
                Expires = DateTime.UtcNow.Add(_jwtSettings.ExpireTime),
                // Asigna las credenciales de firma.
                SigningCredentials = credenciales
            };

            // 5. CREACIÓN Y ESCRITURA DEL TOKEN
            var tokenHandler = new JwtSecurityTokenHandler();

            // Crea el objeto SecurityToken (el token en sí).
            var token = tokenHandler.CreateToken(tokenDescription);

            // Serializa el token a una cadena JWT para ser devuelta al cliente.
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Obtiene el nombre de usuario de la sesión actual (del token JWT ya validado).
        /// </summary>
        /// <returns>El nombre de usuario (NameIdentifier) del JWT.</returns>
        public string GetSessionUser()
        {
            // Accede al contexto HTTP, al usuario autenticado (User), a las claims, 
            // y busca la claim que contiene el identificador de nombre (ClaimTypes.NameIdentifier).
            var username = _httpContextAccessor.HttpContext!.User?.Claims?
                                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            // Se usa '!' asumiendo que el usuario está autenticado y el claim existe.
            return username!;
        }
    }
}
