using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Web.Data;
using InventoryHub.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace InventoryHub.Web.Controllers
{
    public sealed class InventoryCreateVm
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public InventoryCategory Category { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }
}

    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<InventoriesController> _logger;

        public InventoriesController(AppDbContext context, UserManager<IdentityUser> userManager, ILogger<InventoriesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Inventories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Inventories
                .OrderByDescending(i => i.CreatedAtUTC)
                .ToListAsync();
            return View(list);
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
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();
            

            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null)
                return NotFound();
            

            // TODO: Enforce admin or owner access
            return View(inv);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Category,ImageUrl,IsPublic,UpdatedAtUTC")] Inventory invForm)
        {
            if (id != invForm.Id) return NotFound();
            if (!ModelState.IsValid) return View(invForm);

            var inv = await _context.Inventories.FindAsync(id);
            if (inv == null) return NotFound();

            inv.Title = invForm.Title;
            inv.Description = invForm.Description;
            inv.Category = invForm.Category;
            inv.ImageUrl = invForm.ImageUrl;
            inv.IsPublic = invForm.IsPublic;
            inv.UpdatedAtUTC = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id });
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
