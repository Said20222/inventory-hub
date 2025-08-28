using System.Security.Claims;
using InventoryHub.Web.Data;
using InventoryHub.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace InventoryHub.Web.Services
{
    public class AccessService : IAccessService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public AccessService(AppDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<bool> HasWriteAccess(ClaimsPrincipal user, Inventory inv)
        {
            if (!(user.Identity?.IsAuthenticated ?? false))
                return false;

            if (user.IsInRole("Admin"))
                return true;

            var userId = _um.GetUserId(user);
            if (userId == null) return false;

            if (userId == inv.CreatorId)
                return true;

            if (inv.IsPublic)
                return true;

            return await _db.Accesses.AnyAsync(a =>
                a.InventoryId == inv.Id &&
                a.UserId == userId);
        }

        public Task<bool> HasReadAccess(ClaimsPrincipal user, Inventory inv) =>
            Task.FromResult(true);
    }
}