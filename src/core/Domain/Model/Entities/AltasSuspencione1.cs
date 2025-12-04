using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model.Entities;

[Keyless]
[Table("altas_suspenciones", Schema = "comparativo")]
public partial class AltasSuspencione1
{
    [Column("idpais")]
    public int? Idpais { get; set; }

    [Column("idperiodo")]
    public int? Idperiodo { get; set; }

    [Column("contrato")]
    public long? Contrato { get; set; }

    [Column("product_id")]
    public long? ProductId { get; set; }

    [Column("ciclo")]
    public int? Ciclo { get; set; }

    [Column("tipo_cliente")]
    [StringLength(100)]
    public string? TipoCliente { get; set; }

    [Column("des_segmento")]
    [StringLength(100)]
    public string? DesSegmento { get; set; }

    [Column("des_sector")]
    [StringLength(100)]
    public string? DesSector { get; set; }

    [Column("des_categoria")]
    [StringLength(100)]
    public string? DesCategoria { get; set; }

    [Column("des_subcategoria")]
    [StringLength(100)]
    public string? DesSubcategoria { get; set; }

    [Column("idconcepto")]
    public int? Idconcepto { get; set; }

    [Column("concepto")]
    [StringLength(100)]
    public string? Concepto { get; set; }

    [Column("idtipocdr")]
    public int? Idtipocdr { get; set; }

    [Column("clasificacioncdr")]
    [StringLength(100)]
    public string? Clasificacioncdr { get; set; }

    [Column("grupocdr")]
    [StringLength(100)]
    public string? Grupocdr { get; set; }

    [Column("categoriacdr")]
    [StringLength(100)]
    public string? Categoriacdr { get; set; }

    [Column("subcategoriacdr")]
    [StringLength(100)]
    public string? Subcategoriacdr { get; set; }

    [Column("clasecdr")]
    [StringLength(100)]
    public string? Clasecdr { get; set; }

    [Column("subclasecdr")]
    [StringLength(100)]
    public string? Subclasecdr { get; set; }

    [Column("susp_reconec")]
    [StringLength(100)]
    public string? SuspReconec { get; set; }

    [Column("retirado")]
    [StringLength(100)]
    public string? Retirado { get; set; }

    [Column("alta")]
    [StringLength(100)]
    public string? Alta { get; set; }

    [Column("plan_comercial")]
    [StringLength(100)]
    public string? PlanComercial { get; set; }

    [Column("monto_local")]
    public float? MontoLocal { get; set; }

    [Column("monto_usd")]
    public float? MontoUsd { get; set; }

    [Column("velocidad")]
    [StringLength(50)]
    public string? Velocidad { get; set; }

    [Column("velocidad_origen")]
    [StringLength(50)]
    public string? VelocidadOrigen { get; set; }
}
