namespace Infrastructure.Configuration
{
    public static class SapKeywords
    {
        // Palabras clave para detección de encabezados SAP genéricos
        public static readonly string[] GenericSapHeaders =
        {
            "Hora", "Cl.valor", "Usuario", "Proveedor", "Pedido", "CMv", "Txt clase-mov",
            "Material", "Texto breve material", "Ce.", "Alm.", "Doc.mat.", "Texto cab.documento",
            "Lote", "Referencia", "Elemento PEP", "Ce.coste", "Activo fijo", "Reserva",
            "Fe.contab.", "Cantidad", "UMP", "Importe ML", "Valor", "Stock", "UMB", "Mon.",
            "Saldo", "Soc.", "Cta.mayor", "Existenc.", "Desviación", "Visual.líneas", "GrpArtExt", "TpMt", "Año"
        };

        // Palabras clave para detección de headers de saldos
        public static readonly string[] BalanceKeywords =
        {
            "Cta.mayor", "Materiales Mon.", "Saldo al", "Soc.", "Valor total Mon.", "Stock total UMB"
        };

        // Palabras clave para la detección de headers de Doble Línea
        public static readonly string[] DoubleLineKeywords =
        {
            "Material", "Texto", "breve", "Grado", "rotación", "Consumo", "período", "Stock", "medio"
        };

        // 🔹 NUEVO: Patrones obligatorios de Balance (grupos que deben aparecer juntos)
        public static readonly string[][] RequiredBalancePatterns =
        {
            // Patrón 1: Cuentas por Mayor y Moneda
            new[] { "Cta.mayor", "Materiales Mon." }, 
            // Patrón 2: Saldo y Sociedad
            new[] { "Saldo al", "Soc." }
        };

        // 🔹 NUEVO: PATRONES DE DETECCIÓN RÁPIDA (COMBINACIONES OBLIGATORIAS)
        // Usado en DetectSapHeaders para identificar rápidamente el tipo de informe.
        public static readonly string[][] HeaderDetectionPatterns =
        {
            // Patrón 1: Movimientos (Típicamente Hora, Material, Cantidad)
            new[] { "Hora", "Material", "Cantidad" },
            // Patrón 2: Saldos Generales (Típicamente Saldo, Material, Valor)
            new[] { "Saldo", "Material", "Valor" }
        };

        public static readonly string[] StandardBalanceOutputHeaders =
        {
            "Cuenta mayor", "Almacén", "Material", "Stock total", "UMB", "Valor total", "Mon."
        };
    }
}