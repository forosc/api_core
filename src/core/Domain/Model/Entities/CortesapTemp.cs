using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("cortesap_temp")]
public partial class CortesapTemp
{
    [Column("Clase de documento")]
    [StringLength(100)]
    public string? ClaseDeDocumento { get; set; }

    [Column("Nº documento")]
    public long? NºDocumento { get; set; }

    [StringLength(100)]
    public string? Sociedad { get; set; }

    [Column("Ejercicio / mes")]
    [StringLength(100)]
    public string? EjercicioMes { get; set; }

    public long? Cuenta { get; set; }

    [StringLength(100)]
    public string? Texto { get; set; }

    [Column("Sociedad GL asociada")]
    [StringLength(100)]
    public string? SociedadGlAsociada { get; set; }

    [Column("Cuenta de mayor")]
    [StringLength(100)]
    public string? CuentaDeMayor { get; set; }

    [Column("Centro de beneficio")]
    [StringLength(100)]
    public string? CentroDeBeneficio { get; set; }

    [Column("Centro de coste")]
    [StringLength(100)]
    public string? CentroDeCoste { get; set; }

    [Column("Nombre del usuario")]
    [StringLength(100)]
    public string? NombreDelUsuario { get; set; }

    [StringLength(100)]
    public string? Referencia { get; set; }

    [StringLength(100)]
    public string? Factura { get; set; }

    [Column("Referencia a factura")]
    public long? ReferenciaAFactura { get; set; }

    [Column("Fecha de documento")]
    [StringLength(100)]
    public string? FechaDeDocumento { get; set; }

    [Column("Importe en moneda local")]
    public float? ImporteEnMonedaLocal { get; set; }

    [Column("Fe.contabilización")]
    [StringLength(100)]
    public string? FeContabilización { get; set; }

    [Column("Importe en ML2")]
    public float? ImporteEnMl2 { get; set; }

    [Column("Clave clasificación")]
    public int? ClaveClasificación { get; set; }

    [Column("Clave referencia 1")]
    [StringLength(100)]
    public string? ClaveReferencia1 { get; set; }

    [StringLength(100)]
    public string? Asignación { get; set; }

    [Column("Clase cta.contrapar.")]
    [StringLength(100)]
    public string? ClaseCtaContrapar { get; set; }

    [Column("Cta.contrapartida")]
    [StringLength(100)]
    public string? CtaContrapartida { get; set; }

    [Column("Anulado con")]
    public long? AnuladoCon { get; set; }

    [Column("Motivo de anulación")]
    [StringLength(100)]
    public string? MotivoDeAnulación { get; set; }

    [Column("Doc.facturación")]
    [StringLength(50)]
    public string? DocFacturación { get; set; }
}
