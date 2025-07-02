using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("units")]
public partial class Unit
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("shortname")]
    [StringLength(50)]
    public string Shortname { get; set; } = null!;

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
    [InverseProperty("UnitCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Unit")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("Unit")]
    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("UnitUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
