namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class ItemDetailsVm
    {
        public Guid InventoryId { get; set; }
        public Guid ItemId { get; set; }

        public string? CustomId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public int LikeCount { get; set; }
        public bool LikedByMe { get; set; }
    }
}
