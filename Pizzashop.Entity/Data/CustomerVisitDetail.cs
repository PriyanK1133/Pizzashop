using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("customer_visit_details")]
public partial class CustomerVisitDetail
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("section_id")]
    public Guid SectionId { get; set; }

    [Column("customer_id")]
    public Guid CustomerId { get; set; }

    [Column("no_of_persons")]
    public int NoOfPersons { get; set; }

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

    [Column("is_waiting")]
    public bool IsWaiting { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("CustomerVisitDetailCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerVisitDetails")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("CustomerVisitDetails")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("SectionId")]
    [InverseProperty("CustomerVisitDetails")]
    public virtual Section Section { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("CustomerVisitDetailUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
