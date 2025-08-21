namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class InventoryEditVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public InventoryCategory Category { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public uint Version { get; set; }
    }
}