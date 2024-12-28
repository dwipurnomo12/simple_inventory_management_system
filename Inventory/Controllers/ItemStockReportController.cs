using DinkToPdf.Contracts;
using DinkToPdf;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers
{
    public class ItemStockReportController : Controller
    {
        protected readonly AppDbContext _context;
        private string htmlContent;

        public ItemStockReportController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> Index()
        {
            var item = await _context.Items
                .Include(c => c.Category)
                .Include(u => u.Unit)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            return View(item);
        }

        [Authorize(Policy = "superadminOrAdmin")]
        public async Task<IActionResult> GetPdf()
        {
            var data = await _context.Items
                .Include(c => c.Category)
                .Include(u => u.Unit)
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            var htmlContent = @"
            <h1 style='text-align:center; font-family:Arial, sans-serif;'>Item Stock Report</h1>
                <table style='width: 100%; border-collapse: collapse; text-align: left; font-family: Arial, sans-serif;'>
                    <thead style='background-color: #f2f2f2;'>
                        <tr>
                            <th style='padding: 8px; border: 1px solid #ddd;'>No</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Item Name</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Stock</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Category</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Unit</th>
                        </tr>
                    </thead>
                    <tbody>";

                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                htmlContent += $@"
                                            <tr style='border-bottom: 1px solid #ddd;'>
                                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{i + 1}</td>
                                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.ItemName}</td>
                                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.ItemStock}</td>
                                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.Category.CategoryName}</td>
                                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.Unit.UnitName}</td>
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

            return File(pdf, "application/pdf", "Item_Stock_Report.pdf");
        }

    }
}
