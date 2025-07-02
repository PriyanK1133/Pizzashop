using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

public partial class PizzashopContext : DbContext
{
    public PizzashopContext(DbContextOptions<PizzashopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerVisitDetail> CustomerVisitDetails { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemsAndModifierGroup> ItemsAndModifierGroups { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }

    public virtual DbSet<ModifiersAndGroup> ModifiersAndGroups { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderModifier> OrderModifiers { get; set; }

    public virtual DbSet<OrderTaxis> OrderTaxes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesAndPermission> RolesAndPermissions { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableOrderMapping> TableOrderMappings { get; set; }

    public virtual DbSet<TaxesAndFee> TaxesAndFees { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accounts_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsFirstLogin).HasDefaultValueSql("true");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AccountCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accounts_created_by_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accounts_role_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AccountUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accounts_updated_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.AccountUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accounts_user_id_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoryCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("categories_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CategoryUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("categories_updated_by_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cities_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cities_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("countries_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_updated_by_fkey");
        });

        modelBuilder.Entity<CustomerVisitDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_visit_details_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerVisitDetailCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_visit_details_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerVisitDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_visit_details_customer_id_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.CustomerVisitDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_visit_details_section_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerVisitDetailUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_visit_details_updated_by_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsDefaultTax).HasDefaultValueSql("true");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_category_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ItemCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_created_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_unit_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ItemUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_updated_by_fkey");
        });

        modelBuilder.Entity<ItemsAndModifierGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_and_modifier_groups_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ItemsAndModifierGroupCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_and_modifier_groups_created_by_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemsAndModifierGroups).HasConstraintName("items_and_modifier_groups_item_id_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ItemsAndModifierGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_and_modifier_groups_modifier_group_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ItemsAndModifierGroupUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_and_modifier_groups_updated_by_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifiers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_created_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Modifiers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_unit_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModifierUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_updated_by_fkey");
        });

        modelBuilder.Entity<ModifierGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_groups_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierGroupCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_groups_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModifierGroupUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_groups_updated_by_fkey");
        });

        modelBuilder.Entity<ModifiersAndGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifiers_and_groups_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifiersAndGroupCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_and_groups_created_by_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ModifiersAndGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_and_groups_modifier_group__id_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.ModifiersAndGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_and_groups_modifier_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModifiersAndGroupUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_and_groups_updated_by_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_created_by_fkey");

            entity.HasOne(d => d.CustomerVisitDetails).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customer_visit_details_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_updated_by_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderItemCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_created_by_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_item_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderItemUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_updated_by_fkey");
        });

        modelBuilder.Entity<OrderModifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_modifiers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderModifierCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_created_by_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderModifiers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_modifier_id_fkey");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderModifiers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_order_item_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderModifierUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_updated_by_fkey");
        });

        modelBuilder.Entity<OrderTaxis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_taxes_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderTaxisCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_taxes_created_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_taxes_orde_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_taxes_tax_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderTaxisUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_taxes_updated_by_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_created_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_order_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_updated_by_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ratings_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RatingCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_created_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_order_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RatingUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_updated_by_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
        });

        modelBuilder.Entity<RolesAndPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_and_permissions_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RolesAndPermissionCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_and_permissions_created_by_fkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolesAndPermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_and_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolesAndPermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_and_permissions_role_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RolesAndPermissionUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_and_permissions_updated_by_fkey");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sections_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SectionCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sections_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SectionUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sections_updated_by_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("states_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("states_country_id_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_created_by_fkey");

            entity.HasOne(d => d.CurrentOrder).WithMany(p => p.Tables).HasConstraintName("tables_current_order_id_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_section_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TableUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_updated_by_fkey");
        });

        modelBuilder.Entity<TableOrderMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_order_mapping_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableOrderMappingCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_created_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.TableOrderMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_order_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.TableOrderMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_table_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TableOrderMappingUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_updated_by_fkey");
        });

        modelBuilder.Entity<TaxesAndFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("taxes_and_fees_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsEnabled).HasDefaultValueSql("true");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaxesAndFeeCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("taxes_and_fees_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaxesAndFeeUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("taxes_and_fees_updated_by_fkey");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("units_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UnitCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("units_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UnitUpdatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("units_updated_by_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v1()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValueSql("true");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_city_id_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_country_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_created_by_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_state_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_updated_by_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
