namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class InventoryRowVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string OwnerUserName { get; set; } = "";
        public InventoryCategory Category { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAtUTC { get; set; }
    }
}
