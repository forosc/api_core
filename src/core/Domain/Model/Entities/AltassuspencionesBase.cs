using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("altassuspenciones_base")]
[Index("Idperiodo", "Idproducto", "Noo", Name = "altassuspenciones_base_idperiodo_idx")]
[Index("Idproducto", "Idperiodo", Name = "altassuspenciones_base_idproducto_idx")]
public partial class AltassuspencionesBase
{
    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("contrato")]
    public long? Contrato { get; set; }

    [Column("idproducto")]
    public long? Idproducto { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("plancomercial")]
    [StringLength(200)]
    public string? Plancomercial { get; set; }

    [Column("tipocliente")]
    [StringLength(200)]
    public string? Tipocliente { get; set; }

    [Column("dessegmento")]
    [StringLength(200)]
    public string? Dessegmento { get; set; }

    [Column("idconcepto")]
    public int? Idconcepto { get; set; }

    [Column("concepto")]
    [StringLength(200)]
    public string? Concepto { get; set; }

    [Column("noo")]
    [StringLength(200)]
    public string? Noo { get; set; }

    [Column("tiposervicio")]
    [StringLength(200)]
    public string? Tiposervicio { get; set; }

    [Column("familia")]
    [StringLength(50)]
    public string? Familia { get; set; }

    [Column("clasifcdr")]
    [StringLength(200)]
    public string? Clasifcdr { get; set; }

    [Column("cordobas")]
    public decimal? Cordobas { get; set; }

    [Column("dolares")]
    public decimal? Dolares { get; set; }

    [Column("cordobas_ant")]
    public decimal? CordobasAnt { get; set; }

    [Column("dolares_ant")]
    public decimal? DolaresAnt { get; set; }

    [Column("velocidad", TypeName = "character varying")]
    public string? Velocidad { get; set; }

    [Column("velocidad_origen", TypeName = "character varying")]
    public string? VelocidadOrigen { get; set; }
}
