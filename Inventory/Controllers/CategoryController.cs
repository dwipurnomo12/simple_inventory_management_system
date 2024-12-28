using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Index()
        {
            var category = await _context.Categories
                .OrderByDescending(c => c.Id)
                .ToListAsync();
            return View(category);
        }

        [Authorize(Policy = "superadmin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "superadmin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category added successfuly";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Authorize(Policy = "superadmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id == id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category updated successfully !";
                return RedirectToAction("Index");
            }

            return View(category) ;
        }

        [Authorize(Policy = "superadmin")]
        [HttpPost]
        public async Task <IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully !";
            return RedirectToAction("Index");
        }
    }
}
