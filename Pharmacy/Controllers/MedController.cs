using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class MedController : Controller
    {
        private readonly AppDbContext _context;

        public MedController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var medicines = await _context.Medicines
                .Include(m => m.Category)
                .Include(m => m.Supplier)
                .Select(m => new CreateMedViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Packing = m.Packing,
                    GenericName = m.GenericName,
                    SalePrice = m.SalePrice,
                    PrushereDate = m.PrushereDate,
                    Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                    Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : ""
                }).ToListAsync();

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Sup_Name", "Sup_Name");

            return View(medicines);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");

            var model = new CreateMedViewModel
            {
                PrushereDate = DateTime.Now // Default to today's date
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");
                return View(model);
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Cat_Name == model.Cat_Name);
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Sup_Name == model.Sup_Name);

            if (category == null || supplier == null)
            {
                ModelState.AddModelError("", "Invalid category or supplier selected.");
                ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");
                return View(model);
            }

            var medicine = new Med
            {
                Name = model.Name,
                Packing = model.Packing,
                GenericName = model.GenericName,
                ExpiryDate = model.ExpiryDate,
                MRP = model.MRP,
                CategoryId = category.Cat_ID,
                Sup_ID = supplier.Sup_ID,
                PrushereDate = model.PrushereDate,
                SalePrice = model.SalePrice
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            var stock = new MedicineStock
            {
                MedicineId = medicine.Id,
                BatchId = model.BatchId,
                Quantity = model.Quantity
            };

            _context.MedicineStocks.Add(stock);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var med = await (from m in _context.Medicines
                             join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                             from stk in st.DefaultIfEmpty()
                             where m.Id == id
                             select new CreateMedViewModel
                             {
                                 Id = m.Id,
                                 Name = m.Name,
                                 Packing = m.Packing,
                                 GenericName = m.GenericName,
                                 ExpiryDate = m.ExpiryDate,
                                 MRP = m.MRP,
                                 SalePrice = m.SalePrice,
                                 PrushereDate = m.PrushereDate,
                                 Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                                 Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : "",
                                 BatchId = stk != null ? stk.BatchId : "",
                                 Quantity = stk != null ? stk.Quantity : 0
                             }).FirstOrDefaultAsync();

            if (med == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");

            return View(med);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateMedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");
                return View(model);
            }

            var med = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Cat_Name == model.Cat_Name);
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Sup_Name == model.Sup_Name);

            if (category == null || supplier == null)
            {
                ModelState.AddModelError("", "Invalid category or supplier selected.");
                ViewBag.Categories = new SelectList(_context.Categories, "Cat_Name", "Cat_Name");
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Sup_Name", "Sup_Name");
                return View(model);
            }

            med.Name = model.Name;
            med.Packing = model.Packing;
            med.GenericName = model.GenericName;
            med.ExpiryDate = model.ExpiryDate;
            med.MRP = model.MRP;
            med.CategoryId = category.Cat_ID;
            med.Sup_ID = supplier.Sup_ID;
            med.PrushereDate = model.PrushereDate;

            var stock = await _context.MedicineStocks.FirstOrDefaultAsync(s => s.MedicineId == id);
            if (stock != null)
            {
                stock.BatchId = model.BatchId;
                stock.Quantity = model.Quantity;
            }
            else
            {
                _context.MedicineStocks.Add(new MedicineStock
                {
                    MedicineId = med.Id,
                    BatchId = model.BatchId,
                    Quantity = model.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var med = await (from m in _context.Medicines
                             join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                             from stk in st.DefaultIfEmpty()
                             where m.Id == id
                             select new CreateMedViewModel
                             {
                                 Id = m.Id,
                                 Name = m.Name,
                                 Packing = m.Packing,
                                 GenericName = m.GenericName,
                                 ExpiryDate = m.ExpiryDate,
                                 MRP = m.MRP,
                                 SalePrice = m.SalePrice,
                                 PrushereDate = m.PrushereDate,
                                 Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                                 Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : "",
                                 BatchId = stk != null ? stk.BatchId : "",
                                 Quantity = stk != null ? stk.Quantity : 0
                             }).FirstOrDefaultAsync();

            if (med == null) return NotFound();

            return View(med);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var med = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null) return NotFound();

            var stock = await _context.MedicineStocks.FirstOrDefaultAsync(s => s.MedicineId == id);
            if (stock != null)
            {
                _context.MedicineStocks.Remove(stock);
            }

            _context.Medicines.Remove(med);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MedDetails(int id)
        {
            var med = await (from m in _context.Medicines
                             join s in _context.Suppliers on m.Sup_ID equals s.Sup_ID into sup
                             from supplier in sup.DefaultIfEmpty()
                             join c in _context.Categories on m.CategoryId equals c.Cat_ID into cat
                             from category in cat.DefaultIfEmpty()
                             join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                             from stk in st.DefaultIfEmpty()
                             where m.Id == id
                             select new CreateMedViewModel
                             {
                                 Id = m.Id,
                                 Name = m.Name,
                                 Packing = m.Packing,
                                 GenericName = m.GenericName,
                                 ExpiryDate = m.ExpiryDate,
                                 MRP = m.MRP,
                                 SalePrice = m.SalePrice,
                                 PrushereDate = m.PrushereDate,
                                 Cat_Name = category != null ? category.Cat_Name : "",
                                 Sup_Name = supplier != null ? supplier.Sup_Name : "",
                                 BatchId = stk != null ? stk.BatchId : "",
                                 Quantity = stk != null ? stk.Quantity : 0
                             }).FirstOrDefaultAsync();

            if (med == null) return NotFound();

            return View(med);
        }

        public async Task<IActionResult> SearchByName(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var meds = await (from m in _context.Medicines
                              join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                              from stk in st.DefaultIfEmpty()
                              where m.Name.Contains(searchString)
                              select new CreateMedViewModel
                              {
                                  Id = m.Id,
                                  Name = m.Name,
                                  Packing = m.Packing,
                                  GenericName = m.GenericName,
                                  ExpiryDate = m.ExpiryDate,
                                  MRP = m.MRP,
                                  SalePrice = m.SalePrice,
                                  PrushereDate = m.PrushereDate,
                                  Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                                  Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : "",
                                  BatchId = stk != null ? stk.BatchId : "",
                                  Quantity = stk != null ? stk.Quantity : 0
                              }).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Sup_Name", "Sup_Name");

            return View(nameof(Index), meds);
        }

        public async Task<IActionResult> SearchByGenName(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var meds = await (from m in _context.Medicines
                              join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                              from stk in st.DefaultIfEmpty()
                              where m.GenericName.Contains(searchString)
                              select new CreateMedViewModel
                              {
                                  Id = m.Id,
                                  Name = m.Name,
                                  Packing = m.Packing,
                                  GenericName = m.GenericName,
                                  ExpiryDate = m.ExpiryDate,
                                  MRP = m.MRP,
                                  SalePrice = m.SalePrice,
                                  PrushereDate = m.PrushereDate,
                                  Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                                  Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : "",
                                  BatchId = stk != null ? stk.BatchId : "",
                                  Quantity = stk != null ? stk.Quantity : 0
                              }).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Sup_Name", "Sup_Name");

            return View(nameof(Index), meds);
        }

        public async Task<IActionResult> SearchBySupplier(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var meds = await (from m in _context.Medicines
                              join stock in _context.MedicineStocks on m.Id equals stock.MedicineId into st
                              from stk in st.DefaultIfEmpty()
                              join sup in _context.Suppliers on m.Sup_ID equals sup.Sup_ID into supp
                              from supplier in supp.DefaultIfEmpty()
                              where supplier != null && supplier.Sup_Name.Contains(searchString)
                              select new CreateMedViewModel
                              {
                                  Id = m.Id,
                                  Name = m.Name,
                                  Packing = m.Packing,
                                  GenericName = m.GenericName,
                                  ExpiryDate = m.ExpiryDate,
                                  MRP = m.MRP,
                                  SalePrice = m.SalePrice,
                                  PrushereDate = m.PrushereDate,
                                  Cat_Name = m.Category != null ? m.Category.Cat_Name : "",
                                  Sup_Name = m.Supplier != null ? m.Supplier.Sup_Name : "",
                                  BatchId = stk != null ? stk.BatchId : "",
                                  Quantity = stk != null ? stk.Quantity : 0
                              }).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Cat_Name", "Cat_Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Sup_Name", "Sup_Name");

            return View(nameof(Index), meds);
        }
    }
}