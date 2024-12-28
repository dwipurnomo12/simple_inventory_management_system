using DinkToPdf.Contracts;
using DinkToPdf;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers
{
    public class ItemOutReportController : Controller
    {
        protected readonly AppDbContext _context;

        public ItemOutReportController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> GetData([FromQuery] DateOnly? tanggal_mulai, [FromQuery] DateOnly? tanggal_selesai)
        {
            if (!tanggal_mulai.HasValue && tanggal_selesai.HasValue)
            {
                return Json(new { message = "Select date range is required", data = new List<object>() });
            }

            var ItemOut = _context.ItemsOut
                .Include(i => i.Item)
                .Include(c => c.Customer)
                .Where(i => i.DateOfEntry >= tanggal_mulai && i.DateOfEntry <= tanggal_selesai);

            var data = await ItemOut.Select(i => new
            {
                i.Id,
                i.TransactionCode,
                i.DateOfEntry,
                ItemName = i.Item.ItemName,
                i.StockOut,
                CustomerName = i.Customer.CustomerName
            })
            .ToListAsync();

            return Json(data);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> GetPdf([FromQuery] DateOnly? tanggal_mulai, [FromQuery] DateOnly? tanggal_selesai)
        {
            if (!tanggal_mulai.HasValue && tanggal_selesai.HasValue)
            {
                return Json(new { message = "Select date range is required", data = new List<object>() });
            }

            var ItemOut = _context.ItemsOut
                .Include(i => i.Item)
                .Include(c => c.Customer)
                .Where(i => i.DateOfEntry >= tanggal_mulai && i.DateOfEntry <= tanggal_selesai);

            var data = await ItemOut.Select(i => new
            {
                i.Id,
                i.TransactionCode,
                i.DateOfEntry,
                ItemName = i.Item.ItemName,
                i.StockOut,
                CustomerName = i.Customer.CustomerName
            })
            .ToListAsync();

            if (data.Count == 0)
            {
                return Json(new { message = "No data available for the selected date range.", data = new List<object>() });
            }

            var htmlContent = @"
                <h1 style='text-align:center; font-family:Arial, sans-serif;'>Laporan Barang Keluar</h1>
                <h4 style='text-align:center; font-family:Arial, sans-serif;'>Dari Tanggal: " + tanggal_mulai + @" - " + tanggal_selesai + @"</h4>
                <table style='width: 100%; border-collapse: collapse; text-align: left; font-family: Arial, sans-serif;'>
                    <thead style='background-color: #f2f2f2;'>
                        <tr>
                            <th style='padding: 8px; border: 1px solid #ddd;'>No</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Transaction Code</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Date Of Entry</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Item Name</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Stock Out</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Customer</th>
                        </tr>
                    </thead>
                    <tbody>";

                    for (int i = 0; i < data.Count; i++)
                    {
                        var item = data[i];
                        htmlContent += $@"
                                        <tr style='border-bottom: 1px solid #ddd;'>
                                            <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{i + 1}</td>
                                            <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{item.TransactionCode}</td>
                                            <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{item.DateOfEntry:yyyy-MM-dd}</td>
                                            <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.ItemName}</td>
                                            <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.StockOut}</td>
                                            <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.CustomerName}</td>
                                        </tr>";
                    }
                        htmlContent += @"
                    </tbody>
                </table>";

            var pdfConverter = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Out = null
                }
            };

            pdfConverter.Objects.Add(new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8",
                }
            });

            var converter = HttpContext.RequestServices.GetRequiredService<IConverter>();
            var pdf = converter.Convert(pdfConverter);

            return File(pdf, "application/pdf", "Item_out_report.pdf");
        }
    }
}
