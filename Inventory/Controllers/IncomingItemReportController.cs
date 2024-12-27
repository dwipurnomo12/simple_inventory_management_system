using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    public class IncomingItemReportController : Controller
    {
        protected readonly AppDbContext _context;
        
        public IncomingItemReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetData([FromQuery] DateTime? tanggal_mulai, [FromQuery] DateTime? tanggal_selesai)
        {
            var IncomingItem =  _context.IncomingItems
                .Include(s => s.Supplier)
                .Include(i => i.Item)
                .AsQueryable();

            if(tanggal_mulai.HasValue && tanggal_selesai.HasValue)
            {
                IncomingItem = IncomingItem.Where(i => i.DateOfEntry >= tanggal_mulai && i.DateOfEntry <= tanggal_selesai);
            }

            var data = await IncomingItem.Select(i => new
                  {
                      i.Id,
                      i.TransactionCode,
                      i.DateOfEntry,
                      ItemName = i.Item.ItemName,
                      i.StockIn,
                      SupplierName = i.Supplier.SupplierName,
                  })
                  .ToListAsync();

            return Json(data);
        }
    }
}
