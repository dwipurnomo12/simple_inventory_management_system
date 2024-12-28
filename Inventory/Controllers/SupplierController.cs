using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDbContext _context;
        public SupplierController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadmin")]

        public async Task<IActionResult> Index()
        {
            var supplier = await _context.Suppliers
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            return View(supplier);
        }

        [Authorize(Policy = "superadmin")]

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "SupplierName", "SupplierLocation")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Supplier added successfuly";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        [Authorize(Policy = "superadmin")]

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x=>x.Id == id);
            return View(supplier);
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "SupplierName", "SupplierLocation")] Supplier supplier)
        {
            if (ModelState.IsValid) {
                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Supplier updated successfuly";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        [Authorize(Policy = "superadmin")]

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Supplier deleted successfully !";
            return RedirectToAction("Index");
        }
    }
}
