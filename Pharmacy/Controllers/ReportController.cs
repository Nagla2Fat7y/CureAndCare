using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;

namespace Pharmacy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly AppDbContext _dbcontext;

        public ReportController( )
        {
            _dbcontext = new AppDbContext();
        }



        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbcontext.invoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate <= endDate.Value.Date);
            }

            var salesItems = await query
                .Select(i => new SalesItem
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    CustomerName = i.Customer.Name ?? "N/A",
                    NetTotal = i.NetTotal
                })
                .ToListAsync();

            var model = new SalesReportViewModel
            {
                SalesItems = salesItems,
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = salesItems.Sum(item => item.NetTotal)
            };

            return View(model);
        }

        public async Task<IActionResult> InventoryReport(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbcontext.MedicineStocks
                .Include(ms => ms.Med)
                    .ThenInclude(m => m.Supplier)
                .Include(ms => ms.Med)
                    .ThenInclude(m => m.Category)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(ms => ms.Med.ExpiryDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(ms => ms.Med.ExpiryDate <= endDate.Value.Date);
            }

            var stockItems = await query
                .Select(ms => new StockItem
                {
                    MedicineId = ms.MedicineId,
                    MedicineName = ms.Med.Name,
                    GenericName = ms.Med.GenericName,
                    Packing = ms.Med.Packing,
                    ExpiryDate = ms.Med.ExpiryDate,
                    MRP = ms.Med.MRP,
                    PrushereDate = ms.Med.PrushereDate,
                    SalePrice = ms.Med.SalePrice,
                    SupplierName = ms.Med.Supplier != null ? ms.Med.Supplier.Sup_Name : "N/A",
                    CategoryName = ms.Med.Category != null ? ms.Med.Category.Cat_Name : "N/A",
                    BatchId = ms.BatchId,
                    Quantity = ms.Quantity
                })
                .ToListAsync();

            var model = new InventoryReportViewModel
            {
                StockItems = stockItems,
                StartDate = startDate,
                EndDate = endDate,
                TotalQuantity = stockItems.Sum(item => item.Quantity)
            };

            return View(model);
        }
    }

}
