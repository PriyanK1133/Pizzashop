using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("order_items")]
public partial class OrderItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("item_id")]
    public Guid ItemId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("special_instruction")]
    public string? SpecialInstruction { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("tax")]
    public decimal Tax { get; set; }

    [Column("total_modifier_amount")]
    [Precision(10, 2)]
    public decimal TotalModifierAmount { get; set; }

    [Column("item_total")]
    [Precision(10, 2)]
    public decimal ItemTotal { get; set; }

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

    [Column("ready_quantity")]
    public int? ReadyQuantity { get; set; }

    [Column("item_name")]
    [StringLength(50)]
    public string ItemName { get; set; } = null!;

    [Column("item_rate")]
    [Precision(10, 2)]
    public decimal ItemRate { get; set; }

    [Column("total_amount")]
    [Precision(10, 2)]
    public decimal TotalAmount { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderItemCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("OrderItems")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [InverseProperty("OrderItem")]
    public virtual ICollection<OrderModifier> OrderModifiers { get; set; } = new List<OrderModifier>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("OrderItemUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
