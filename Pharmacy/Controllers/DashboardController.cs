using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var model = new DashboardViewModel
            {
                MedicineCount = await _context.Medicines.CountAsync(),
                SupplierCount = await _context.Suppliers.CountAsync(),
                CustomerCount = await _context.Customers.CountAsync(),
                InvoiceCount = await _context.invoices.CountAsync(),
                OutOfStockCount = await _context.MedicineStocks.CountAsync(m => m.Quantity <= 0),
                LowStockCount = await _context.MedicineStocks.CountAsync(m => m.Quantity <= 10), // Adjustable threshold
                ExpiredCount = await _context.Medicines.CountAsync(m => m.ExpiryDate < DateTime.Now),
                
                TotalSales = await _context.invoices
                    .Where(i => i.InvoiceDate.Date == today)
                    .SumAsync(i => i.NetTotal),
               
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult OutOfStock()
        {
            var model = _context.MedicineStocks
                .Where(m => m.Quantity <= 0)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Category)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Supplier)
                .Select(m => new OutOfStockViewModel
                {
                    MedicineName = m.Med.Name,
                    GenericName = m.Med.GenericName,
                    Packing = m.Med.Packing,
                    CategoryName = m.Med.Category != null ? m.Med.Category.Cat_Name : "No Category",
                    SupplierName = m.Med.Supplier != null ? m.Med.Supplier.Sup_Name : "No Supplier",
                    ExpiryDate = m.Med.ExpiryDate,
                    SalePrice = m.Med.SalePrice,
                    MRP = m.Med.MRP,
                    BatchId = m.BatchId,
                    Quantity= m.Quantity,
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult LowStock()
        {
            var model = _context.MedicineStocks
                .Where(m => m.Quantity <= 10)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Category)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Supplier)
                .Select(m => new OutOfStockViewModel
                {
                    MedicineName = m.Med.Name,
                    GenericName = m.Med.GenericName,
                    Packing = m.Med.Packing,
                    CategoryName = m.Med.Category != null ? m.Med.Category.Cat_Name : "No Category",
                    SupplierName = m.Med.Supplier != null ? m.Med.Supplier.Sup_Name : "No Supplier",
                    ExpiryDate = m.Med.ExpiryDate,
                    SalePrice = m.Med.SalePrice,
                    MRP = m.Med.MRP,
                    BatchId = m.BatchId,
                    Quantity = m.Quantity,
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult Expired()
        {
            var model = _context.MedicineStocks
                .Where(m => m.Med.ExpiryDate <DateTime.Now)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Category)
                .Include(m => m.Med)
                    .ThenInclude(med => med.Supplier)
                .Select(m => new OutOfStockViewModel
                {
                    MedicineName = m.Med.Name,
                    GenericName = m.Med.GenericName,
                    Packing = m.Med.Packing,
                    CategoryName = m.Med.Category != null ? m.Med.Category.Cat_Name : "No Category",
                    SupplierName = m.Med.Supplier != null ? m.Med.Supplier.Sup_Name : "No Supplier",
                    ExpiryDate = m.Med.ExpiryDate,
                    SalePrice = m.Med.SalePrice,
                    MRP = m.Med.MRP,
                    BatchId = m.BatchId,
                    Quantity = m.Quantity,
                })
                .ToList();

            return View(model);
        }



    }
}