using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("items_and_modifier_groups")]
public partial class ItemsAndModifierGroup
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("item_id")]
    public Guid? ItemId { get; set; }

    [Column("modifier_group_id")]
    public Guid ModifierGroupId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [Column("min_selection")]
    public int MinSelection { get; set; }

    [Column("max_selection")]
    public int MaxSelection { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ItemsAndModifierGroupCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemsAndModifierGroups")]
    public virtual Item? Item { get; set; }

    [ForeignKey("ModifierGroupId")]
    [InverseProperty("ItemsAndModifierGroups")]
    public virtual ModifierGroup ModifierGroup { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ItemsAndModifierGroupUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
