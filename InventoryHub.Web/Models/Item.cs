using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;

        [Required, MaxLength(128)]
        public string CustomId { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string CreatorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ItemFieldValue> Values { get; set; } = new List<ItemFieldValue>();
        [Timestamp]
        public uint Version { get; set; } // This property is used for concurrency control with optimistic locking
    }
}