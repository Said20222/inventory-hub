using System.Security.Claims;
using InventoryHub.Web.Models;

namespace InventoryHub.Web.Services
{
    public interface IAccessService
    {
        Task<bool> HasWriteAccess(ClaimsPrincipal user, Inventory inventory);
        Task<bool> HasReadAccess(ClaimsPrincipal user, Inventory inventory);
    }
}