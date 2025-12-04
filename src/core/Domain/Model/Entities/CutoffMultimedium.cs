using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("cutoff_multimedia")]
public partial class CutoffMultimedium
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("concepto")]
    [StringLength(100)]
    public string? Concepto { get; set; }

    [Column("estimacion")]
    public float? Estimacion { get; set; }

    [Column("reversion")]
    public float? Reversion { get; set; }

    [Column("facturacion")]
    public float? Facturacion { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("idperiodo")]
    public long? Idperiodo { get; set; }
}
