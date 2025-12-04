using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.POCO
{
    // Proyecto: DataProcessor.Domain/Models/CorteSapData.cs

    public record CorteSapData
    {
        // Propiedades de cadena limpias
        public string? ClaseDeDocumento { get; init; }
        public long? NumeroDocumento { get; init; } // Nombre limpio
        public string? Sociedad { get; init; }
        public string? EjercicioMes { get; init; }
        public long? Cuenta { get; init; }
        public string? Texto { get; init; }
        public string? SociedadGlAsociada { get; init; }
        public string? CuentaDeMayor { get; init; }
        public string? CentroDeBeneficio { get; init; }
        public string? CentroDeCoste { get; init; }
        public string? NombreDelUsuario { get; init; }
        public string? Referencia { get; init; }
        public string? Factura { get; init; }
        public long? ReferenciaAFactura { get; init; }

        // Propiedades de datos que deben ser parseadas (ej. fechas y floats)
        // Se recomienda usar tipos nativos (DateTime, decimal/float) para el dominio,
        // pero si los datos de entrada son Strings, se mantienen String? y se parsean en Application/Infrastructure.
        public string? FechaDeDocumento { get; init; }
        public float? ImporteEnMonedaLocal { get; init; }
        public string? FeContabilizacion { get; init; }
        public float? ImporteEnMl2 { get; init; }

        // Propiedades numéricas
        public int? ClaveClasificacion { get; init; }
        public string? ClaveReferencia1 { get; init; }
        public string? Asignacion { get; init; }
        public string? ClaseCtaContrapar { get; init; }
        public string? CtaContrapartida { get; init; }
        public long? AnuladoCon { get; init; }
        public string? MotivoDeAnulacion { get; init; }
        public string? DocFacturacion { get; init; }
    }
}
