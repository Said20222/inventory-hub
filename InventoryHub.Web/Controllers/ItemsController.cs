using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Web.Data;
using InventoryHub.Web.Models;
using InventoryHub.Web.Models.ViewModels;

namespace InventoryHub.Web.Controllers
{
    [Authorize]
    [Route("Inventories/{inventoryId:guid}/Items")]
    public class ItemsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(AppDbContext dbContext, UserManager<IdentityUser> userManager, ILogger<ItemsController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute]Guid inventoryId)
        {
            var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();

            var items = await _dbContext.Items
                .Where(i => i.InventoryId == inventoryId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var canWrite = CanWrite(inventory);

            ViewBag.inventory = inventory;
            ViewBag.canWrite = canWrite;
            return View(items);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(Guid inventoryId)
        {
            var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();

            if (!CanWrite(inventory)) return Forbid();

            var vm = new ItemCreateVm { InventoryId = inventoryId };
            return View(vm);
        }

        [HttpPost("Create"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid inventoryId, ItemCreateVm vm)
        {
            if (inventoryId != vm.InventoryId) return NotFound();

            var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();
            if (!CanWrite(inventory)) return Forbid();

            if (!ModelState.IsValid) return View(vm);

            var entity = new Item
            {
                Id = Guid.NewGuid(),
                InventoryId = vm.InventoryId,
                CustomId = string.IsNullOrWhiteSpace(vm.CustomId)
                    ? Guid.NewGuid().ToString("N")[..9] 
                    : vm.CustomId.Trim(),
                CreatorId = _userManager.GetUserId(User)!,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Items.Add(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { inventoryId, id = entity.Id });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                ModelState.AddModelError(nameof(vm.CustomId), "Custom ID must be unique within this inventory.");
                return View(vm);
            }
        }

        [HttpGet("{id:guid}/Edit")]
        public async Task<IActionResult> Edit(Guid inventoryId, Guid id) {
            var item = await _dbContext.Items
                .Include(i => i.Inventory)
                .FirstOrDefaultAsync(i => i.Id == id && i.InventoryId == inventoryId);
            if (item == null) return NotFound();

            var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();
            if (!CanWrite(inventory)) return Forbid();

            var vm = new ItemEditVm
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                CustomId = item.CustomId,
                Version = item.Version
            };

            return View(vm);
        }

        [HttpPost("{id:guid}/Edit"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid inventoryId, Guid id, ItemEditVm vm)
        {
            if (id != vm.Id || inventoryId != vm.InventoryId) return NotFound();

            var item = await _dbContext.Items
                .Include(i => i.Inventory)
                .FirstOrDefaultAsync(i => i.Id == id && i.InventoryId == inventoryId);
            if (item == null) return NotFound();

            var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();
            if (!CanWrite(item.Inventory)) return Forbid();

            if (!ModelState.IsValid) return View(vm);

            _dbContext.Entry(item).Property(i => i.Version).OriginalValue = vm.Version;

            item.CustomId = vm.CustomId.Trim();
            item.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { inventoryId, id = item.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "The item was modified by another user. Please reload and try again.");
                return View(vm);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                ModelState.AddModelError(nameof(vm.CustomId), "Custom ID must be unique within this inventory.");
                return View(vm);
            }
        }

        // For testing only - remove later to use inventoryAccess
        private bool CanWrite(Inventory inv)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return false;
            var userId = _userManager.GetUserId(User);
            if (userId == inv.CreatorId) return true;
            return User.IsInRole("Admin");
        }
    }
}
