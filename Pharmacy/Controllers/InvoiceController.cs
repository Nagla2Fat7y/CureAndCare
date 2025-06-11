using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pharmacy.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.invoices
                .Include(i => i.Customer)
                .Include(i => i.emp)
                .Select(i => new InvoiceViewModel
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    CustomerName = i.Customer.Name,
                    EmployeeName = i.emp != null ? i.emp.UserName : "غير محدد",
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    TotalDiscount = i.TotalDiscount,
                    NetTotal = i.NetTotal
                })
                .ToListAsync();

            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.invoices
                .Include(i => i.Customer)
                .Include(i => i.emp)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Med)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new InvoiceViewModel
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer.Name,
                EmployeeName = invoice.emp?.UserName ?? "Not Specified",
                InvoiceDate = invoice.InvoiceDate,
                PaymentType = invoice.PaymentType,
                TotalAmount = invoice.TotalAmount,
                TotalDiscount = invoice.TotalDiscount,
                NetTotal = invoice.NetTotal,
                PaidAmount = invoice.PaidAmount,
                Change = invoice.Change,
                InvoiceItems = invoice.Items.Select(ii => new InvoiceItemViewModel
                {
                    InvoiceItemId = ii.InvoiceItemId,
                    MedicineName = ii.Med.Name,
                    MedId = ii.MedId,
                    Quantity = ii.Quantity,
                    Expiry = ii.Med.ExpiryDate.ToString("yyyy-MM"),
                    PrushereDate = ii.PrushereDate,
                    MRP = (double)ii.Med.MRP,
                    SalePrice = (double)ii.SalePrice,
                    DiscountPercentage = ii.DiscountPercentage,
                    Total = ii.Total
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new InvoiceViewModel
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                InvoiceDate = DateTime.Now
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.InvoiceItems == null || !model.InvoiceItems.Any())
            {
                ModelState.AddModelError("", "يجب إضافة دواء واحد على الأقل إلى الفاتورة.");
                return View(model);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Name == model.CustomerName);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = model.CustomerName,
                    Add = model.Address,
                    Phone = model.ContactNumber
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            var employee = await _context.emps
                .FirstOrDefaultAsync(e => e.UserName == model.EmployeeName);

            if (employee == null)
            {
                ModelState.AddModelError("", "الموظف غير موجود.");
                return View(model);
            }

            var invoice = new Invoice
            {
                InvoiceNumber = model.InvoiceNumber,
                InvoiceDate = model.InvoiceDate,
                PaymentType = model.PaymentType,
                CustomerId = customer.Customer_Id,
                TotalAmount = model.TotalAmount,
                TotalDiscount = model.TotalDiscount,
                NetTotal = model.NetTotal,
                PaidAmount = model.PaidAmount,
                Change = model.Change,
                empid = employee.Id
            };

            _context.invoices.Add(invoice);
            await _context.SaveChangesAsync();

            foreach (var item in model.InvoiceItems)
            {
                var medicine = await _context.Medicines
                    .FirstOrDefaultAsync(m => m.Name == item.MedicineName);

                if (medicine == null)
                {
                    ModelState.AddModelError("", $"الدواء غير موجود: {item.MedicineName} - {item.BatchId}");
                    return View(model);
                }

                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = invoice.InvoiceId,
                    MedId = medicine.Id,
                    Quantity = item.Quantity,
                    BatchId = item.BatchId,
                    MRP = (decimal)item.MRP,
                    SalePrice = (double)item.SalePrice,
                    PrushereDate = item.PrushereDate,

                    DiscountPercentage = item.DiscountPercentage,
                    Total = item.Total
                };

                _context.InvoiceItems.Add(invoiceItem);

                var stock = await _context.MedicineStocks
                    .FirstOrDefaultAsync(s => s.MedicineId == medicine.Id && s.BatchId == item.BatchId);

                if (stock == null || stock.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"الكمية المطلوبة غير متوفرة للدواء: {item.MedicineName} - {item.BatchId}");
                    return View(model);
                }

                stock.Quantity -= item.Quantity;
                _context.MedicineStocks.Update(stock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetMedicineDetails(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new List<object>());
            }

            var medicines = _context.Medicines
                .Where(m => m.Name.Contains(name))
                .Select(m => new
                {
                    medicineName = m.Name,
                    batchId = m.MedicineStocks.FirstOrDefault().BatchId,
                    availableQty = _context.MedicineStocks
                        .Where(s => s.MedicineId == m.Id && s.BatchId == m.MedicineStocks.FirstOrDefault().BatchId)
                        .Select(s => s.Quantity)
                        .FirstOrDefault(),
                    expiry = m.ExpiryDate.ToString("yyyy-MM-dd"),
                    salePrice = m.SalePrice
                })
                .Take(10)
                .ToList();

            return Json(medicines);
        }

        [HttpGet]
        public IActionResult GetCustomerDetails(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new List<object>());
            }

            var customers = _context.Customers
                .Where(c => c.Name.Contains(name))
                .Select(c => new
                {
                    customerName = c.Name,
                    address = c.Add,
                    contactNumber = c.Phone
                })
                .Take(10)
                .ToList();

            return Json(customers);
        }

        [HttpGet]
        public IActionResult GetEmployeeDetails(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new List<object>());
            }

            var employees = _context.emps
                .Where(e => e.UserName.Contains(name))
                .Select(e => new
                {
                    employeeName = e.UserName
                })
                .Take(10)
                .ToList();

            return Json(employees);
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _context.invoices
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefault();
            int nextNumber = lastInvoice != null ? int.Parse(lastInvoice.InvoiceNumber.Replace("INV", "")) + 1 : 1;
            return $"INV{nextNumber:D4}";
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevenueReport(string filterType = "Day", string reportType = "Sales")
        {
            var model = new RevenueFilterViewModel
            {
                FilterType = filterType,
                ReportType = reportType
            };

            var query = _context.invoices
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Med)
                .Where(i => i.Items.Any());

            switch (filterType.ToLower())
            {
                case "day":
                    model.RevenueData = query
                        .AsEnumerable()
                        .GroupBy(i => i.InvoiceDate.Date)
                        .Select(g =>
                        {
                            var periodDate = g.Key;

                            var sales = g.Sum(i => i.NetTotal);
                            var totalDiscount = g.Sum(i => i.TotalDiscount);
                            var profit = g.Sum(i => i.Items.Sum(ii =>
                                (decimal)(ii.SalePrice - ii.Med.MRP) * ii.Quantity)) - totalDiscount;

                            return new RevenueReportViewModel
                            {
                                Period = periodDate.ToString("yyyy-MM-dd"),
                                Sales = sales,
                                PurchaseCost = g.Sum(i => i.Items.Sum(ii => (decimal)(ii.Med.MRP * ii.Quantity))),
                                Profit = profit
                            };
                        })
                        .OrderBy(r => r.Period)
                        .ToList();
                    break;

                case "month":
                    model.RevenueData = query
                        .AsEnumerable()
                        .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                        .Select(g =>
                        {
                            var year = g.Key.Year;
                            var month = g.Key.Month;

                            var sales = g.Sum(i => i.NetTotal);
                            var totalDiscount = g.Sum(i => i.TotalDiscount);
                            var profit = g.Sum(i => i.Items.Sum(ii =>
                                (decimal)(ii.SalePrice - ii.Med.MRP) * ii.Quantity)) - totalDiscount;

                            return new RevenueReportViewModel
                            {
                                Period = $"{year}-{month:D2}",
                                Sales = sales,
                                PurchaseCost = g.Sum(i => i.Items.Sum(ii => (decimal)(ii.Med.MRP * ii.Quantity))),
                                Profit = profit
                            };
                        })
                        .OrderBy(r => r.Period)
                        .ToList();
                    break;

                case "year":
                    model.RevenueData = query
                        .AsEnumerable()
                        .GroupBy(i => i.InvoiceDate.Year)
                        .Select(g =>
                        {
                            var year = g.Key;

                            var sales = g.Sum(i => i.NetTotal);
                            var totalDiscount = g.Sum(i => i.TotalDiscount);
                            var profit = g.Sum(i => i.Items.Sum(ii =>
                                (decimal)(ii.SalePrice - ii.Med.MRP) * ii.Quantity)) - totalDiscount;

                            return new RevenueReportViewModel
                            {
                                Period = year.ToString(),
                                Sales = sales,
                                PurchaseCost = g.Sum(i => i.Items.Sum(ii => (decimal)(ii.Med.MRP * ii.Quantity))),
                                Profit = profit
                            };
                        })
                        .OrderBy(r => r.Period)
                        .ToList();
                    break;

                default:
                    model.RevenueData = new List<RevenueReportViewModel>();
                    break;
            }

            return View(model);
        }



    }
}
  
