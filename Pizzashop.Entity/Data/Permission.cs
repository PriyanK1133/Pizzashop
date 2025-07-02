using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("permissions")]
public partial class Permission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("preference")]
    public int? Preference { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<RolesAndPermission> RolesAndPermissions { get; set; } = new List<RolesAndPermission>();
}
