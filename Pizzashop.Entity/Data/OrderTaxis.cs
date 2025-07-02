using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("order_taxes")]
public partial class OrderTaxis
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("tax_id")]
    public Guid TaxId { get; set; }

    [Column("tax_value")]
    [Precision(10, 2)]
    public decimal TaxValue { get; set; }

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

    [Column("tax_name")]
    [StringLength(20)]
    public string TaxName { get; set; } = null!;

    [Column("tax_type")]
    [StringLength(20)]
    public string TaxType { get; set; } = null!;

    [Column("total_tax")]
    [Precision(10, 2)]
    public decimal TotalTax { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderTaxisCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderTaxes")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("TaxId")]
    [InverseProperty("OrderTaxes")]
    public virtual TaxesAndFee Tax { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("OrderTaxisUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
