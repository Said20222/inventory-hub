namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class InventoryCreateVm
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public InventoryCategory Category { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
    }
}
