using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

[Table("users")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
[Index("Phone", Name = "users_phone_key", IsUnique = true)]
[Index("UserName", Name = "users_username_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("user_name")]
    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("profile_image")]
    public string? ProfileImage { get; set; }

    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("city_id")]
    public Guid CityId { get; set; }

    [Column("state_id")]
    public Guid StateId { get; set; }

    [Column("country_id")]
    public Guid CountryId { get; set; }

    [Column("zipcode")]
    [StringLength(50)]
    public string? Zipcode { get; set; }

    [Column("phone")]
    [StringLength(15)]
    public string Phone { get; set; } = null!;

    [Required]
    [Column("is_active")]
    public bool? IsActive { get; set; }

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

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Account> AccountCreatedByNavigations { get; set; } = new List<Account>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Account> AccountUpdatedByNavigations { get; set; } = new List<Account>();

    [InverseProperty("User")]
    public virtual ICollection<Account> AccountUsers { get; set; } = new List<Account>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; } = new List<Category>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Category> CategoryUpdatedByNavigations { get; set; } = new List<Category>();

    [ForeignKey("CityId")]
    [InverseProperty("Users")]
    public virtual City City { get; set; } = null!;

    [ForeignKey("CountryId")]
    [InverseProperty("Users")]
    public virtual Country Country { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("InverseCreatedByNavigation")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Customer> CustomerUpdatedByNavigations { get; set; } = new List<Customer>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<CustomerVisitDetail> CustomerVisitDetailCreatedByNavigations { get; set; } = new List<CustomerVisitDetail>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<CustomerVisitDetail> CustomerVisitDetailUpdatedByNavigations { get; set; } = new List<CustomerVisitDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Item> ItemCreatedByNavigations { get; set; } = new List<Item>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Item> ItemUpdatedByNavigations { get; set; } = new List<Item>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ItemsAndModifierGroup> ItemsAndModifierGroupCreatedByNavigations { get; set; } = new List<ItemsAndModifierGroup>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<ItemsAndModifierGroup> ItemsAndModifierGroupUpdatedByNavigations { get; set; } = new List<ItemsAndModifierGroup>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Modifier> ModifierCreatedByNavigations { get; set; } = new List<Modifier>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ModifierGroup> ModifierGroupCreatedByNavigations { get; set; } = new List<ModifierGroup>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<ModifierGroup> ModifierGroupUpdatedByNavigations { get; set; } = new List<ModifierGroup>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Modifier> ModifierUpdatedByNavigations { get; set; } = new List<Modifier>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ModifiersAndGroup> ModifiersAndGroupCreatedByNavigations { get; set; } = new List<ModifiersAndGroup>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<ModifiersAndGroup> ModifiersAndGroupUpdatedByNavigations { get; set; } = new List<ModifiersAndGroup>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Order> OrderCreatedByNavigations { get; set; } = new List<Order>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<OrderItem> OrderItemCreatedByNavigations { get; set; } = new List<OrderItem>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<OrderItem> OrderItemUpdatedByNavigations { get; set; } = new List<OrderItem>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<OrderModifier> OrderModifierCreatedByNavigations { get; set; } = new List<OrderModifier>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<OrderModifier> OrderModifierUpdatedByNavigations { get; set; } = new List<OrderModifier>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<OrderTaxis> OrderTaxisCreatedByNavigations { get; set; } = new List<OrderTaxis>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<OrderTaxis> OrderTaxisUpdatedByNavigations { get; set; } = new List<OrderTaxis>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Order> OrderUpdatedByNavigations { get; set; } = new List<Order>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Payment> PaymentCreatedByNavigations { get; set; } = new List<Payment>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Payment> PaymentUpdatedByNavigations { get; set; } = new List<Payment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Rating> RatingCreatedByNavigations { get; set; } = new List<Rating>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Rating> RatingUpdatedByNavigations { get; set; } = new List<Rating>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RolesAndPermission> RolesAndPermissionCreatedByNavigations { get; set; } = new List<RolesAndPermission>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<RolesAndPermission> RolesAndPermissionUpdatedByNavigations { get; set; } = new List<RolesAndPermission>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Section> SectionCreatedByNavigations { get; set; } = new List<Section>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Section> SectionUpdatedByNavigations { get; set; } = new List<Section>();

    [ForeignKey("StateId")]
    [InverseProperty("Users")]
    public virtual State State { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Table> TableCreatedByNavigations { get; set; } = new List<Table>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TableOrderMapping> TableOrderMappingCreatedByNavigations { get; set; } = new List<TableOrderMapping>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<TableOrderMapping> TableOrderMappingUpdatedByNavigations { get; set; } = new List<TableOrderMapping>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Table> TableUpdatedByNavigations { get; set; } = new List<Table>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TaxesAndFee> TaxesAndFeeCreatedByNavigations { get; set; } = new List<TaxesAndFee>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<TaxesAndFee> TaxesAndFeeUpdatedByNavigations { get; set; } = new List<TaxesAndFee>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Unit> UnitCreatedByNavigations { get; set; } = new List<Unit>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<Unit> UnitUpdatedByNavigations { get; set; } = new List<Unit>();

    [ForeignKey("UpdatedBy")]
    [InverseProperty("InverseUpdatedByNavigation")]
    public virtual User UpdatedByNavigation { get; set; } = null!;
}
