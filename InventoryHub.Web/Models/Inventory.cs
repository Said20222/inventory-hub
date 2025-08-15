using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.Models
{

    public enum InventoryCategory
    {
        Equipment,
        Furniture,
        Book,
        Other
    }
    public class Inventory
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public InventoryCategory Category { get; set; } = InventoryCategory.Other;
        [MaxLength(1024)]
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        [Required]
        public string CreatorId { get; set; } = string.Empty;
        public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUTC { get; set; }

        public ICollection<InventoryTag> Tags { get; set; } = new List<InventoryTag>();
        public ICollection<InventoryAccess> Accesses { get; set; } = new List<InventoryAccess>();
        public ICollection<Field> Fields { get; set; } = new List<Field>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        // This property is used for concurrency control with optimistic locking
        [Timestamp]
        public uint Version { get; set; }

    }
}