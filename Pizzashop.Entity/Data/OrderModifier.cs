using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("order_modifiers")]
public partial class OrderModifier
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_item_id")]
    public Guid OrderItemId { get; set; }

    [Column("modifier_id")]
    public Guid ModifierId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

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

    [Column("modifier_name")]
    [StringLength(50)]
    public string ModifierName { get; set; } = null!;

    [Column("modifier_rate")]
    public decimal ModifierRate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderModifierCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifierId")]
    [InverseProperty("OrderModifiers")]
    public virtual Modifier Modifier { get; set; } = null!;

    [ForeignKey("OrderItemId")]
    [InverseProperty("OrderModifiers")]
    public virtual OrderItem OrderItem { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("OrderModifierUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
