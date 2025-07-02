using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("accounts")]
public partial class Account
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    [Column("token")]
    public Guid? Token { get; set; }

    [Column("token_expiry", TypeName = "timestamp without time zone")]
    public DateTime? TokenExpiry { get; set; }

    [Required]
    [Column("is_first_login")]
    public bool? IsFirstLogin { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("AccountCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Accounts")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("AccountUpdatedByNavigations")]
    public virtual User UpdatedByNavigation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AccountUsers")]
    public virtual User User { get; set; } = null!;
}
