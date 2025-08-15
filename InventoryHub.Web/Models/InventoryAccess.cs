namespace InventoryHub.Web.Models
{
    public class InventoryAccess
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;

        // TODO: Add a role enum for access control
    }
}