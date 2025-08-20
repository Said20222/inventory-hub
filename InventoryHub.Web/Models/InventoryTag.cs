namespace InventoryHub.Web.Models
{
    public class InventoryTag
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;
        public Guid TagId { get; set; }
        public Tag? Tag { get; set; } = default;
    }
}