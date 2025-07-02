using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("cities")]
public partial class City
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("state_id")]
    public Guid StateId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey("StateId")]
    [InverseProperty("Cities")]
    public virtual State State { get; set; } = null!;

    [InverseProperty("City")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
