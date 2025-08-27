using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace InventoryHub.Web.Models
{
    public class InventoryAccess
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;
        
        [Required]

        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = default!;

        // TODO: Add a role enum for access control
    }
}