using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("cdr_resumen", Schema = "indicadores")]
public partial class CdrResuman
{
    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("claro")]
    public long? Claro { get; set; }

    [Column("tigo")]
    public long? Tigo { get; set; }

    [Column("segundos")]
    public long? Segundos { get; set; }

    [Column("minutos")]
    public decimal? Minutos { get; set; }

    [Column("costominuto")]
    public decimal? Costominuto { get; set; }

    [Column("costo")]
    public decimal? Costo { get; set; }
}
