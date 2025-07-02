using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("taxes_and_fees")]
public partial class TaxesAndFee
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Required]
    [Column("is_enabled")]
    public bool? IsEnabled { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; }

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

    [Column("tax_amount")]
    public decimal TaxAmount { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TaxesAndFeeCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Tax")]
    public virtual ICollection<OrderTaxis> OrderTaxes { get; set; } = new List<OrderTaxis>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("TaxesAndFeeUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
