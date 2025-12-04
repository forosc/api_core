using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("sapresumen", Schema = "indicadores")]
public partial class Sapresuman
{
    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("cuenta")]
    public long? Cuenta { get; set; }

    [Column("nodocumento")]
    public long? Nodocumento { get; set; }

    [Column("centrobeneficio", TypeName = "character varying")]
    public string? Centrobeneficio { get; set; }

    [Column("idconcepto")]
    public int? Idconcepto { get; set; }

    [Column("concepto", TypeName = "character varying")]
    public string? Concepto { get; set; }

    [Column("asignacion")]
    [StringLength(100)]
    public string? Asignacion { get; set; }

    [Column("referencia", TypeName = "character varying")]
    public string? Referencia { get; set; }

    [Column("claveclasificacion")]
    public int? Claveclasificacion { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("rentacordoba")]
    public double? Rentacordoba { get; set; }

    [Column("renta")]
    public double? Renta { get; set; }

    [Column("clasedocumento", TypeName = "character varying")]
    public string? Clasedocumento { get; set; }

    [Column("documentodescripcion")]
    [StringLength(100)]
    public string? Documentodescripcion { get; set; }

    [Column("fechadocumento")]
    public DateOnly? Fechadocumento { get; set; }

    [Column("fecontabilizacion")]
    public DateOnly? Fecontabilizacion { get; set; }

    [Column("tipo")]
    [StringLength(100)]
    public string? Tipo { get; set; }

    [Column("clasif_0")]
    [StringLength(100)]
    public string? Clasif0 { get; set; }

    [Column("clasif_1")]
    [StringLength(100)]
    public string? Clasif1 { get; set; }

    [Column("clasif_2")]
    [StringLength(100)]
    public string? Clasif2 { get; set; }

    [Column("clasif_3")]
    [StringLength(100)]
    public string? Clasif3 { get; set; }

    [Column("factor")]
    public int? Factor { get; set; }

    [Column("clasif_4")]
    [StringLength(100)]
    public string? Clasif4 { get; set; }

    [Column("tiposervicio")]
    public string? Tiposervicio { get; set; }

    [Column("esintercompania")]
    [StringLength(10)]
    public string? Esintercompania { get; set; }
}
