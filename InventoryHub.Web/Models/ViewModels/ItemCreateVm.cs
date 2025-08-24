namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class ItemCreateVm
    {
        public Guid InventoryId { get; set; }
        public string CustomId { get; set; } = "";
        // TODO: Add remaining dynamic field values
    }
}