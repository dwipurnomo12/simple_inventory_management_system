using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "CustomerName", "CustomerLocation")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer added successfuly";
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "CustomerName", "CustomerLocation")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer updated successfully !";
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if(customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer deleted successfully !";
            return RedirectToAction("Index");
        }
    }
}
