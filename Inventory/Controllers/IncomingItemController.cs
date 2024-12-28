using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class IncomingItemController : Controller
    {
        protected readonly AppDbContext _context;
        private readonly ILogger<IncomingItemController> _logger;
        public IncomingItemController(AppDbContext context, ILogger<IncomingItemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> Index()
        {
            var incomingItem = await _context.IncomingItems
                .Include(s => s.Supplier)
                .Include(i => i.Item)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            return View(incomingItem);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public IActionResult Create()
        {
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName");
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "SupplierName");
            return View();
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id", "ItemId", "DateOfEntry", "StockIn", "SupplierId")] IncomingItem incomingItem)
        {
            if (ModelState.IsValid)
            {
                var random = new Random();
                int randomNumber = random.Next(10000, 99999);
                incomingItem.TransactionCode = $"INV-{randomNumber}";

                _context.Add(incomingItem);
                await _context.SaveChangesAsync();

                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == incomingItem.ItemId);
                if (item != null)
                {
                    item.ItemStock += incomingItem.StockIn;

                    _context.Items.Update(item);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Incoming Item added successfuly";
                return RedirectToAction("Index");
            }
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName");
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "SupplierName");
            return View(incomingItem);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task <IActionResult> Edit(int id)
        {
            var incomingItem = await _context.IncomingItems.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName");
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "SupplierName");

            return View(incomingItem);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "ItemId", "DateOfEntry", "StockIn", "SupplierId")] IncomingItem incomingItem)
        {
            var existingStockIn = await _context.IncomingItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (existingStockIn == null)
            {
                return NotFound();
            }

            incomingItem.TransactionCode = existingStockIn.TransactionCode;
            int stockDifference = incomingItem.StockIn - existingStockIn.StockIn;

            _context.Update(incomingItem);
            await _context.SaveChangesAsync();

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == incomingItem.ItemId);
            if (item != null)
            {
                item.ItemStock += stockDifference;
                if (item.ItemStock < 0)
                {
                    TempData["ErrorMessage"] = "Stock cannot be negative ";

                    ViewBag.Items = new SelectList(_context.Items.ToList(), "Id", "ItemName", incomingItem.ItemId);
                    ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "SupplierName", incomingItem.SupplierId);
                    return View(incomingItem);
                }
                
                _context.Items.Update(item);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Incoming Item updated successfuly";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "superadminOrAdmin")]
        [HttpPost]
        public async Task <IActionResult> Delete(int id)
        {
            var incomingItem = await _context.IncomingItems.FindAsync(id);
            if(incomingItem == null)
            {
                return NotFound();
            }

            _context.IncomingItems.Remove(incomingItem);
            await _context.SaveChangesAsync();

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == incomingItem.ItemId);
            if (item != null)
            {
                item.ItemStock -= incomingItem.StockIn;

                _context.Items.Update(item);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Incoming Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
