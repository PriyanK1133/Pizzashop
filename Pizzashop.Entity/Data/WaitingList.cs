using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("waiting_list")]
public partial class WaitingList
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("customer_visit_details_id")]
    public Guid CustomerVisitDetailsId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("WaitingListCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerVisitDetailsId")]
    [InverseProperty("WaitingLists")]
    public virtual CustomerVisitDetail CustomerVisitDetails { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("WaitingListUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
