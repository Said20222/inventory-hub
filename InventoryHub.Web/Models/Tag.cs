using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.Models
{
    public class Tag
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }
}