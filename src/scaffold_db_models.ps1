# ====================================================================
# SCRIPT DE AUTOMATIZACIÓN PARA SCAFFOLDING (INGENIERÍA INVERSA)
# Genera clases de modelo y DbContext a partir de una Base de Datos existente.
# Ejecutar desde la línea de comandos/terminal: .\scaffold_db_models.ps1
# ====================================================================

# 1. Variables de Configuración
# CADENA DE CONEXIÓN
$DB_CONNECTION = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Claro2024"
# PROVEEDOR DE BASE DE DATOS
$DB_PROVIDER = "Npgsql.EntityFrameworkCore.PostgreSQL"
# PROYECTO DE INICIO (Configuración, inyección de dependencias)
$STARTUP_PROJECT= "src/api_core"
# PROYECTO OBJETIVO (El proyecto que ejecuta el comando)
$PROJECT_TARGET = "src/infrastructure"

# NOMBRE DE TU DB CONTEXT EXISTENTE (¡CRÍTICO!)
# Esto le dice a EF Core que el contexto ya existe y que solo genere las entidades.
$EXISTING_DB_CONTEXT_NAME = "DataDBContext" 

# CARPETA DE SALIDA (Ubicación de las ENTIDADES, relativa al PROJECT_TARGET)
# Ruta relativa: subir de src/infrastructure (..) e ir a core/Domain/Model/Entities
$OUTPUT_DIR = "../core/Domain/Model/Entities"

# LISTA DE TABLAS A GENERAR (separadas por coma)
$TABLES = "Clientes, Pedidos, Productos"
# 2. Comando Principal de Scaffolding

Write-Host "Iniciando Scaffold de Entity Framework Core..."

# Comando 'dotnet ef dbcontext scaffold'
# --force: Sobrescribe los archivos existentes sin pedir confirmación.
# --data-annotations: Usa atributos [Required], [MaxLength], etc., en lugar de Fluent API.

dotnet ef dbcontext scaffold $DB_CONNECTION `
    $DB_PROVIDER `
    --startup-project $STARTUP_PROJECT `
    --project $PROJECT_TARGET `
    --output-dir $OUTPUT_DIR `
    --tables $TABLES `
    --context $EXISTING_DB_CONTEXT_NAME `
    --force `
    --data-annotations

# 3. Verificación de Resultados
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Scaffolding completado con éxito. Archivos generados en el Dominio." -ForegroundColor Green
    Write-Host "---------------------------------------------------------"
    Write-Host "⚠️ PASO MANUAL CRÍTICO REQUERIDO:" -ForegroundColor Yellow
    Write-Host "1. Localiza el archivo DbContext (ej. 'TuProyectoContext.cs') en la carpeta:" -ForegroundColor Yellow
    Write-Host "   src/core/Domain/Model/Entities/" -ForegroundColor Yellow
    Write-Host "2. MUEVE ese archivo al destino final:" -ForegroundColor Yellow
    Write-Host "   src/infrastructure/Context/" -ForegroundColor Yellow
    Write-Host "3. Corrige el Namespace del archivo DbContext movido." -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "❌ Error durante el Scaffolding. Revisa los mensajes anteriores." -ForegroundColor Red
}

Write-Host ""
Write-Host "---------------------------------------------------------"