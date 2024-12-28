using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class ItemController : Controller
    {
        protected readonly AppDbContext _context;
        private readonly ILogger<ItemController> _logger;
        public ItemController(AppDbContext context, ILogger<ItemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Index()
        {
            var item = await _context.Items
                .Include(c => c.Category)
                .Include(u => u.Unit)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            return View(item);
        }

        [Authorize(Policy = "superadmin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
            ViewBag.Units = new SelectList(_context.Units.ToList(), "Id", "UnitName");
            return View();
        }

        [Authorize(Policy = "superadmin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "ItemName", "ItemDescription", "CategoryId", "UnitId")] Item item, IFormFile ItemImage)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Model Error: {ErrorMessage}", error.ErrorMessage);
                }
            }

            // If validation is successful
            if (ModelState.IsValid)
            {
                // Storage folder location
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/item-image");

                // Create a folder if it doesn't exist yet
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ItemImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save to directory
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ItemImage.CopyToAsync(fileStream);
                }

                // Save to path model
                item.ItemImage = $"/item-image/{uniqueFileName}";

                // Save to database
                item.CreatedAt = DateTime.Now;
                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Item added successfully.";
                return RedirectToAction("Index");
            }

            // if failed valisation, return back into form
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
            ViewBag.Units = new SelectList(_context.Units.ToList(), "Id", "UnitName");
            return View(item);
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
            ViewBag.Units = new SelectList(_context.Units.ToList(), "Id", "UnitName");

            return View(item) ;
        }

        [Authorize(Policy = "superadmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "ItemName", "ItemDescription", "CategoryId", "UnitId")] Item item, IFormFile ItemImage)
        {
            if (item == null)
            {
                return NotFound();
            }

            if (ItemImage != null && ItemImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/item-image");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ItemImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ItemImage.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(item.ItemImage))
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ItemImage.TrimStart('/'));

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                }
                item.ItemImage = $"/item-image/{uniqueFileName}";
            }
            else
            {
                var existingItem = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
                if (existingItem != null)
                {
                    item.ItemImage = existingItem.ItemImage; 
                }
            }

            _context.Update(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item updated successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
            ViewBag.Units = new SelectList(_context.Units.ToList(), "Id", "UnitName");

            return View(item);
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(item.ItemImage))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ItemImage.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
