using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("modifiers_and_groups")]
public partial class ModifiersAndGroup
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("modifier_id")]
    public Guid ModifierId { get; set; }

    [Column("modifier_group__id")]
    public Guid ModifierGroupId { get; set; }

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
    [InverseProperty("ModifiersAndGroupCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifierId")]
    [InverseProperty("ModifiersAndGroups")]
    public virtual Modifier Modifier { get; set; } = null!;

    [ForeignKey("ModifierGroupId")]
    [InverseProperty("ModifiersAndGroups")]
    public virtual ModifierGroup ModifierGroup { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ModifiersAndGroupUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
