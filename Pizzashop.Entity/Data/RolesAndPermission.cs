using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("roles_and_permissions")]
public partial class RolesAndPermission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("permission_id")]
    public Guid PermissionId { get; set; }

    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Column("can_view")]
    public bool CanView { get; set; }

    [Column("can_edit")]
    public bool CanEdit { get; set; }

    [Column("can_delete")]
    public bool CanDelete { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("RolesAndPermissionCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("RolesAndPermissions")]
    public virtual Permission Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("RolesAndPermissions")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("RolesAndPermissionUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
