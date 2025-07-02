using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_date")]
    public DateOnly OrderDate { get; set; }

    [Column("customer_visit_details_id")]
    public Guid CustomerVisitDetailsId { get; set; }

    [Column("discount")]
    [Precision(3, 2)]
    public decimal Discount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("subtotal")]
    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    [Column("tax")]
    public decimal Tax { get; set; }

    [Column("order_total")]
    [Precision(10, 2)]
    public decimal OrderTotal { get; set; }

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

    [Column("invoice_number")]
    [StringLength(50)]
    public string? InvoiceNumber { get; set; }

    [Column("order_served_time", TypeName = "timestamp without time zone")]
    public DateTime? OrderServedTime { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerVisitDetailsId")]
    [InverseProperty("Orders")]
    public virtual CustomerVisitDetail CustomerVisitDetails { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderTaxis> OrderTaxes { get; set; } = new List<OrderTaxis>();

    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Order")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("Order")]
    public virtual ICollection<TableOrderMapping> TableOrderMappings { get; set; } = new List<TableOrderMapping>();

    [InverseProperty("CurrentOrder")]
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("OrderUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
