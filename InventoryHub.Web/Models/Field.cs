using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.Models
{
    public enum FieldType
    {
        SingleLine,
        MultiLine,
        Numeric,
        Url, 
        Boolean
    }
    public class Field
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public FieldType Type { get; set; }
        public bool ShowInTable { get; set; }
        public int Order { get; set; }
    }
}