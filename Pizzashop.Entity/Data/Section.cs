using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("sections")]
public partial class Section
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
    [InverseProperty("SectionCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Section")]
    public virtual ICollection<CustomerVisitDetail> CustomerVisitDetails { get; set; } = new List<CustomerVisitDetail>();

    [InverseProperty("Section")]
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("SectionUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
