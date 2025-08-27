using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Web.Models;


namespace InventoryHub.Web.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<InventoryTag> InventoryTags => Set<InventoryTag>();
        public DbSet<InventoryAccess> Accesses => Set<InventoryAccess>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<ItemFieldValue> Values => Set<ItemFieldValue>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inventory>(e =>
            {
                e.ToTable("Inventories");
                e.Property(i => i.Title).IsRequired().HasMaxLength(200);
                e.Property(i => i.Description).HasMaxLength(1000);
                e.Property(i => i.ImageUrl).HasMaxLength(1024);
                e.Property(i => i.CreatorId).IsRequired();
            });

            modelBuilder.Entity<Item>(e =>
            {
                e.ToTable("Items");
                e.Property(i => i.CustomId).IsRequired().HasMaxLength(128);
                e.Property(i => i.CreatorId).IsRequired();
                e.HasOne(i => i.Inventory)
                    .WithMany(inv => inv.Items)
                    .HasForeignKey(i => i.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(i => new { i.InventoryId, i.CustomId }).IsUnique();
            });

            modelBuilder.Entity<Tag>(e =>
            {
                e.ToTable("Tags");
                e.HasIndex(t => t.Name).IsUnique();
                e.Property(t => t.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<InventoryTag>(e =>
            {
                e.ToTable("InventoryTags");
                e.HasKey(it => new { it.InventoryId, it.TagId });
                e.HasOne(it => it.Inventory)
                    .WithMany(i => i.Tags)
                    .HasForeignKey(it => it.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(it => it.Tag)
                    .WithMany(t => t.InventoryTags)
                    .HasForeignKey(it => it.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InventoryAccess>(e =>
            {
                e.ToTable("InventoryAccess");
                e.HasKey(ia => new { ia.InventoryId, ia.UserId });
                e.HasIndex(ia => new { ia.UserId, ia.InventoryId }).IsUnique();

                e.HasOne(ia => ia.Inventory)
                    .WithMany(i => i.Accesses)
                    .HasForeignKey(ia => ia.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                e.HasOne(ia => ia.User)
                    .WithMany()
                    .HasForeignKey(ia => ia.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Field>(e =>
            {
                e.ToTable("Fields");
                e.Property(f => f.Name).IsRequired().HasMaxLength(200);
                e.Property(f => f.Description).HasMaxLength(1000);
                e.HasOne(f => f.Inventory)
                    .WithMany(i => i.Fields)
                    .HasForeignKey(f => f.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(f => new { f.InventoryId, f.Order });
            });
            
            modelBuilder.Entity<ItemFieldValue>(e =>
            {
                e.ToTable("ItemFieldValues");

                e.HasOne(v => v.Item)
                    .WithMany(i => i.Values)
                    .HasForeignKey(v => v.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(v => v.Field)
                    .WithMany()
                    .HasForeignKey(v => v.FieldId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                e.HasIndex(x => new { x.ItemId, x.FieldId }).IsUnique();
            });
        }
    }
}