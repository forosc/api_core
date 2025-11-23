using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class InventoryDBContextFactory : IDesignTimeDbContextFactory<DataDBContext>
    {
        /// <summary>
        /// Crea una instancia del DbContext en tiempo de diseño.
        /// Este método es llamado automáticamente por las herramientas de Entity Framework Core (dotnet ef).
        /// </summary>
        /// <param name="args">Argumentos pasados por la herramienta EF Core (normalmente vacíos).</param>
        /// <returns>Una instancia configurada de InventoryDBContext.</returns>
        public DataDBContext CreateDbContext(string[] args)
        {
            // 1. CARGA MANUAL DE LA CONFIGURACIÓN (settings files)

            // Crea un objeto de configuración (ConfigurationBuilder) ya que la DI del host no está activa.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                // Establece la ruta base para buscar los archivos de configuración (generalmente la carpeta del proyecto).
                .SetBasePath(Directory.GetCurrentDirectory())
                // Carga el archivo de configuración principal.
                .AddJsonFile("appsettings.json")
                // Carga el archivo de configuración de desarrollo (opcional), sobrescribiendo los valores anteriores.
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // 2. OBTENCIÓN DE LA CADENA DE CONEXIÓN

            // Lee la cadena de conexión del archivo de configuración cargado. 
            // Se asume que la clave de la cadena de conexión es "DefaultConnection".
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. CONSTRUCCIÓN DE LAS OPCIONES DEL DB CONTEXT

            // Objeto utilizado para configurar el proveedor de base de datos.
            var optionsBuilder = new DbContextOptionsBuilder<DataDBContext>();

            // Configura EF Core para usar el proveedor PostgreSQL (o cualquier otro) 
            // e inyecta la cadena de conexión obtenida.
            optionsBuilder.UseNpgsql(connectionString); // O UseSqlServer, UseSqlite, etc.

            // 4. INSTANCIACIÓN DEL DB CONTEXT

            // Crea y devuelve una nueva instancia del DbContext utilizando las opciones configuradas.
            return new DataDBContext(optionsBuilder.Options);
        }
    }
}
