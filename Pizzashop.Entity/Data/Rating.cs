using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("ratings")]
public partial class Rating
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("food_rating")]
    [Precision(2, 1)]
    public decimal FoodRating { get; set; }

    [Column("service_rating")]
    [Precision(2, 1)]
    public decimal ServiceRating { get; set; }

    [Column("ambience_rating")]
    [Precision(2, 1)]
    public decimal AmbienceRating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("RatingCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Ratings")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("RatingUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
