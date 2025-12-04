using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("altassuspencionesconceptos", Schema = "indicadores")]
public partial class Altassuspencionesconcepto
{
    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("plancomercial")]
    [StringLength(200)]
    public string? Plancomercial { get; set; }

    [Column("idconcepto")]
    public int? Idconcepto { get; set; }

    [Column("concepto")]
    [StringLength(200)]
    public string? Concepto { get; set; }

    [Column("tiposervicio")]
    [StringLength(200)]
    public string? Tiposervicio { get; set; }

    [Column("noo")]
    [StringLength(200)]
    public string? Noo { get; set; }

    [Column("renta")]
    public decimal? Renta { get; set; }

    [Column("segmentocliente")]
    [StringLength(50)]
    public string? Segmentocliente { get; set; }

    [Column("renta_ant")]
    public decimal? RentaAnt { get; set; }
}
