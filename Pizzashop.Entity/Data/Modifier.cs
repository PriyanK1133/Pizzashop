using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("modifiers")]
public partial class Modifier
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("rate")]
    [Precision(7, 2)]
    public decimal Rate { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_id")]
    public Guid UnitId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

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

    [ForeignKey("CreatedBy")]
    [InverseProperty("ModifierCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Modifier")]
    public virtual ICollection<ModifiersAndGroup> ModifiersAndGroups { get; set; } = new List<ModifiersAndGroup>();

    [InverseProperty("Modifier")]
    public virtual ICollection<OrderModifier> OrderModifiers { get; set; } = new List<OrderModifier>();

    [ForeignKey("UnitId")]
    [InverseProperty("Modifiers")]
    public virtual Unit Unit { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ModifierUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
