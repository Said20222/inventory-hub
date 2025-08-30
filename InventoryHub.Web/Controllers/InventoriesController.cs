using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Web.Data;
using InventoryHub.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using InventoryHub.Web.Models.ViewModels;
using InventoryHub.Web.Services;

namespace InventoryHub.Web.Controllers
{


    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccessService _access;

        public InventoriesController(AppDbContext context, UserManager<IdentityUser> userManager, IAccessService access)
        {
            _context = context;
            _userManager = userManager;
            _access = access;
        }

        // GET: Inventories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var rows = await
                (from i in _context.Inventories
                 join u in _context.Users on i.CreatorId equals u.Id
                 orderby i.CreatedAtUTC descending
                 select new InventoryRowVm
                 {
                     Id = i.Id,
                     Title = i.Title,
                     OwnerUserName = u.UserName!,
                     Category = i.Category,
                     IsPublic = i.IsPublic,
                     CreatedAtUTC = i.CreatedAtUTC
                 }).ToListAsync();

            return View(rows);
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryCreateVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var entity = new Inventory
            {
                Id = Guid.NewGuid(),
                Title = vm.Title,
                Description = vm.Description,
                Category = vm.Category,
                ImageUrl = vm.ImageUrl,
                IsPublic = vm.IsPublic,
                CreatedAtUTC = DateTime.UtcNow,
                CreatorId = _userManager.GetUserId(User)!
            };

            _context.Inventories.Add(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = entity.Id });
        }


        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await _access.HasWriteAccess(User, inv)) return Forbid();


            var vm = new InventoryEditVm
            {
                Id = inv.Id,
                Title = inv.Title,
                Description = inv.Description,
                Category = inv.Category,
                ImageUrl = inv.ImageUrl,
                IsPublic = inv.IsPublic,
                Version = inv.Version
            };
            return View(vm);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, InventoryEditVm vm)
        {
            if (id != vm.Id) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await _access.HasWriteAccess(User, inv)) return Forbid();

            _context.Entry(inv).Property(i => i.Version).OriginalValue = vm.Version;

            inv.Title = vm.Title;
            inv.Description = vm.Description;
            inv.Category = vm.Category;
            inv.ImageUrl = vm.ImageUrl;
            inv.IsPublic = vm.IsPublic;
            inv.UpdatedAtUTC = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "The inventory was updated by another user. Please reload the page and try again.");
                return View(vm);
            }
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inv = await _context.Inventories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inv == null) return NotFound();
            if (!await _access.HasWriteAccess(User, inv)) return Forbid();

            return View(inv);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null && await _access.HasWriteAccess(User, inventory))
            {
                _context.Inventories.Remove(inventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet("Inventories/{id:guid}/Access")]
        public async Task<IActionResult> Access(Guid id, [FromServices] IAccessService access, [FromServices] UserManager<IdentityUser> um)
        {
            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await access.HasWriteAccess(User, inv)) return Forbid();

            var writers = await _context.Accesses
                .Where(a => a.InventoryId == id)
                .Join(_context.Users, a => a.UserId, u => u.Id, (a,u) => new InventoryAccessVm.UserRow {
                    UserId = u.Id, UserName = u.UserName!, Email = u.Email
                })
                .OrderBy(x => x.UserName)
                .ToListAsync();

            var vm = new InventoryAccessVm {
                InventoryId = id,
                InventoryTitle = inv.Title,
                IsPublic = inv.IsPublic,
                Writers = writers
            };
            return View(vm);
        }

        [HttpPost("Inventories/{id:guid}/Access/Add"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWriter(Guid id, InventoryAccessVm vm, [FromServices] IAccessService access, [FromServices] UserManager<IdentityUser> um)
        {
            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await access.HasWriteAccess(User, inv)) return Forbid();

            if (string.IsNullOrWhiteSpace(vm.Query))
                return RedirectToAction(nameof(Access), new { id });

            // Find by email first, then by username
            var user = await um.FindByEmailAsync(vm.Query) ?? await um.FindByNameAsync(vm.Query);
            if (user == null) {
                TempData["AccessError"] = "User not found.";
                return RedirectToAction(nameof(Access), new { id });
            }

            if (user.Id == inv.CreatorId) {
                TempData["AccessError"] = "Owner already has full access.";
                return RedirectToAction(nameof(Access), new { id });
            }

            var exists = await _context.Accesses.AnyAsync(a => a.InventoryId == id && a.UserId == user.Id);
            if (!exists)
            {
                _context.Accesses.Add(new InventoryAccess { InventoryId = id, UserId = user.Id });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Access), new { id });
        }

        [HttpPost("Inventories/{id:guid}/Access/Remove"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveWriter(Guid id, string userId, [FromServices] IAccessService access)
        {
            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await access.HasWriteAccess(User, inv)) return Forbid();

            var row = await _context.Accesses.FindAsync(id, userId);
            if (row != null)
            {
                _context.Accesses.Remove(row);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Access), new { id });
        }

        [HttpPost("Inventories/{id:guid}/Access/Public"), ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublic(Guid id, bool isPublic, [FromServices] IAccessService access)
        {
            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (!await access.HasWriteAccess(User, inv)) return Forbid();

            inv.IsPublic = isPublic;
            inv.UpdatedAtUTC = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Access), new { id });
        }

    }
}
