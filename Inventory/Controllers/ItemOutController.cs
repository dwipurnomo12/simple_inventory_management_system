using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class ItemOutController : Controller
    {
        protected readonly AppDbContext _context;
        public ItemOutController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> Index()
        {
            var itemOut = await _context.ItemsOut
                .Include(i => i.Item)
                .Include(c => c.Customer)
                .ToListAsync();
            return View(itemOut);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public IActionResult Create()
        {
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName");
            ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "CustomerName");
            return View();
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "ItemId", "DateOfEntry", "StockOut", "CustomerId")] ItemOut itemOut)
        {
            if (ModelState.IsValid)
            {
                var random = new Random();
                int randomNumber = random.Next(10000, 99999);
                itemOut.TransactionCode = $"INV-{randomNumber}";

                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemOut.ItemId);
                if (item == null)
                {
                    ModelState.AddModelError("ItemId", "The selected item does not exist.");
                    ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", itemOut.ItemId);
                    return View(itemOut);
                }

                if (itemOut.StockOut > item.ItemStock)
                {
                    ModelState.AddModelError("StockOut", "Stock out cannot exceed available ItemStock.");
                    ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", itemOut.ItemId);
                    ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "CustomerName", itemOut.CustomerId);
                    return View(itemOut);
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    {
                        _context.Add(itemOut);
                        await _context.SaveChangesAsync();

                        item.ItemStock -= itemOut.StockOut;
                        _context.Items.Update(item);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = "Item successfully added.";
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", itemOut.ItemId);
            ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "CustomerName", itemOut.CustomerId);
            return View(itemOut);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var itemOut = await _context.ItemsOut.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName");
            ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "CustomerName");

            return View(itemOut);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "ItemId", "DateOfEntry", "StockOut", "CustomerId")] ItemOut itemOut)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", itemOut.ItemId);
                ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "SupplierName", itemOut.CustomerId);
                return View(itemOut);
            }

            var existingStockOut = await _context.ItemsOut.FirstOrDefaultAsync(x => x.Id == id);
            if (existingStockOut == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemOut.ItemId);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Item not found.";
                return RedirectToAction("Index");
            }

            int stockDifference = itemOut.StockOut - existingStockOut.StockOut;

            if (item.ItemStock - stockDifference < 0)
            {
                TempData["ErrorMessage"] = "Stock is insufficient for the requested update.";

                ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", itemOut.ItemId);
                ViewBag.Customers = new SelectList(_context.Customers.ToList(), "Id", "SupplierName", itemOut.CustomerId);
                return View(itemOut);
            }

            item.ItemStock -= stockDifference;
            _context.Items.Update(item);

            _context.Entry(existingStockOut).CurrentValues.SetValues(itemOut);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item Out updated successfully";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var itemOut = await _context.ItemsOut.FindAsync(id);
            if (itemOut == null)
            {
                return NotFound();
            }

            _context.ItemsOut.Remove(itemOut);
            await _context.SaveChangesAsync();

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemOut.ItemId);
            if (item != null)
            {
                item.ItemStock += itemOut.StockOut;

                _context.Items.Update(item);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Incoming Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
