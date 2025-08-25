using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class ItemEditVm
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        
        [Required, MaxLength(128)]
        public string CustomId { get; set; } = "";
        public uint Version { get; set; }

        // TODO: Add remaining dynamic field values
    }
}