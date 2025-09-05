using Microsoft.AspNetCore.Identity;

namespace InventoryHub.Web.Models
{
    public class ItemLike
    {
        public Guid ItemId { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Item Item { get; set; } = default!;
        public IdentityUser User { get; set; } = default!;
    }
}
