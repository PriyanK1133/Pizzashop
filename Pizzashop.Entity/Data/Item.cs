using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("items")]
public partial class Item
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Column("rate")]
    [Precision(9, 2)]
    public decimal Rate { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_id")]
    public Guid UnitId { get; set; }

    [Column("shortcode")]
    [StringLength(50)]
    public string? Shortcode { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Required]
    [Column("is_default_tax")]
    public bool? IsDefaultTax { get; set; }

    [Column("tax_percentage")]
    [Precision(4, 2)]
    public decimal TaxPercentage { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("is_favourite")]
    public bool IsFavourite { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Items")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("ItemCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Item")]
    public virtual ICollection<ItemsAndModifierGroup> ItemsAndModifierGroups { get; set; } = new List<ItemsAndModifierGroup>();

    [InverseProperty("Item")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("UnitId")]
    [InverseProperty("Items")]
    public virtual Unit Unit { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("ItemUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
