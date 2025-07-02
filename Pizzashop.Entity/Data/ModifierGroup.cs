using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("modifier_groups")]
public partial class ModifierGroup
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

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

    [Column("preference")]
    public int? Preference { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ModifierGroupCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("ModifierGroup")]
    public virtual ICollection<ItemsAndModifierGroup> ItemsAndModifierGroups { get; set; } = new List<ItemsAndModifierGroup>();

    [InverseProperty("ModifierGroup")]
    public virtual ICollection<ModifiersAndGroup> ModifiersAndGroups { get; set; } = new List<ModifiersAndGroup>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ModifierGroupUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
