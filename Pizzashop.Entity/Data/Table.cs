using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("tables")]
public partial class Table
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("capacity")]
    public short Capacity { get; set; }

    [Column("is_occupied")]
    public bool IsOccupied { get; set; }

    [Column("section_id")]
    public Guid SectionId { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [Column("current_order_id")]
    public Guid? CurrentOrderId { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TableCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CurrentOrderId")]
    [InverseProperty("Tables")]
    public virtual Order? CurrentOrder { get; set; }

    [ForeignKey("SectionId")]
    [InverseProperty("Tables")]
    public virtual Section Section { get; set; } = null!;

    [InverseProperty("Table")]
    public virtual ICollection<TableOrderMapping> TableOrderMappings { get; set; } = new List<TableOrderMapping>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("TableUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
