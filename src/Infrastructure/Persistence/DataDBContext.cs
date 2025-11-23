using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    // Nota: Reemplaza 'YourProjectNamespace.Domain.Entities' con el namespace real de tu clase User.

    /// <summary>
    /// Contexto de la base de datos para la aplicación. 
    /// Hereda de IdentityDbContext<User> para incluir automáticamente todas las tablas requeridas por ASP.NET Identity 
    /// (ej. AspNetUsers, AspNetRoles, etc.).
    /// </summary>
    public class DataDBContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Constructor requerido por Entity Framework Core.
        /// Recibe las opciones de configuración del contexto (ej. cadena de conexión, proveedor de DB).
        /// </summary>
        /// <param name="options">Opciones del contexto de la base de datos.</param>
        public DataDBContext(DbContextOptions<DataDBContext> options)
            : base(options)
        {
        }

        /*
        // --- DB SETS (Serán definidos más adelante) ---
        // Cuando definas tus entidades de dominio, las agregarás aquí:

        // public DbSet<Product> Products { get; set; }
        // public DbSet<Category> Categories { get; set; }

        // NOTA: No es necesario definir DbSet<User> ya que IdentityDbContext lo maneja automáticamente.
        */

        /// <summary>
        /// Método llamado cuando se están creando o configurando las tablas (Migraciones).
        /// Se utiliza para aplicar configuraciones específicas de modelos, restricciones, índices, 
        /// y la configuración de las tablas de Identity.
        /// </summary>
        /// <param name="modelBuilder">Objeto usado para construir el modelo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // LLAMADA BASE: Es OBLIGATORIA al heredar de IdentityDbContext<TUser>
            // Esto configura el esquema y las tablas de ASP.NET Identity.
            base.OnModelCreating(modelBuilder);

            /*
            // --- CONFIGURACIÓN DE ENTIDADES PERSONALIZADAS ---
            // Aquí puedes aplicar configuraciones específicas a tus entidades de dominio:

            // Ejemplo de configuración de la clave compuesta o índices:
            // modelBuilder.Entity<Product>().HasKey(p => p.Id);

            // Ejemplo para aplicar configuraciones desde clases separadas:
            // modelBuilder.ApplyConfiguration(new ProductConfiguration());
            */

        }
    }
}
