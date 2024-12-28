using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class UnitController : Controller
    {
        protected readonly AppDbContext _context;
        public UnitController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadmin")]

        public async Task<IActionResult> Index()
        {
            var unit = await _context.Units
                .OrderByDescending(u => u.Id)
                .ToListAsync();
            return View(unit);
        }

        [Authorize(Policy = "superadmin")]

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "UnitName")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Units.Add(unit);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Unit added successfully";
                return RedirectToAction("Index");
            }
            return View(unit);
        }

        [Authorize(Policy = "superadmin")]

        public async Task<IActionResult> Edit(int id)
        {
            var unit = await _context.Units.FirstOrDefaultAsync(x=>x.Id == id);
            return View(unit);
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "UnitName")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Update(unit);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Unit updated successfully";
                return RedirectToAction("Index");
            }
            return View(unit);
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if(unit == null)
            {
                return NotFound();
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Unit deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
