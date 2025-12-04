using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("altassuspenciones", Schema = "indicadores")]
public partial class Altassuspencione
{
    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("plancomercial")]
    [StringLength(200)]
    public string? Plancomercial { get; set; }

    [Column("contrato")]
    public long? Contrato { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("noo")]
    [StringLength(200)]
    public string? Noo { get; set; }

    [Column("tipocliente")]
    [StringLength(200)]
    public string? Tipocliente { get; set; }

    [Column("segmentocliente")]
    [StringLength(50)]
    public string? Segmentocliente { get; set; }

    [Column("tiposervicio")]
    [StringLength(200)]
    public string? Tiposervicio { get; set; }

    [Column("familia")]
    [StringLength(50)]
    public string? Familia { get; set; }

    [Column("cantidad")]
    public long? Cantidad { get; set; }

    [Column("renta")]
    public decimal? Renta { get; set; }

    [Column("ifi")]
    [StringLength(10)]
    public string? Ifi { get; set; }
}
