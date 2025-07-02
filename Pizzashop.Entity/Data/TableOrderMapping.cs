using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("table_order_mapping")]
public partial class TableOrderMapping
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("table_id")]
    public Guid TableId { get; set; }

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TableOrderMappingCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("TableOrderMappings")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("TableId")]
    [InverseProperty("TableOrderMappings")]
    public virtual Table Table { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("TableOrderMappingUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
