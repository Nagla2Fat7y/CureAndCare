using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;

namespace Pharmacy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class empController : Controller
    {
        private readonly UserManager<emp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public empController(UserManager<emp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = new AppDbContext();
        }

        private bool EmployeeExists(string id)
        {
            return _context.emps.Any(e => e.Id == id);
        }

        // ------------------- Index -------------------
        public IActionResult Index()
        {
            var emps = _context.emps.ToList();
            return View(emps);
        }

        // ------------------- Create -------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(emp emp)
        {
            if (!ModelState.IsValid)
                return View(emp);

            _context.emps.Add(emp);
            _context.SaveChanges();
            TempData["Success"] = "Employee added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ------------------- Edit -------------------
        // GET: emp/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var pharmacist = await _context.Users.FindAsync(id);
            if (pharmacist == null)
                return NotFound();

            return View(pharmacist);
        }

        // POST: emp/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, emp model, string? NewPassword, string? ConfirmPassword)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "هناك خطأ في البيانات المدخلة.";
                return View(model);
            }

            var pharmacist = await _userManager.FindByIdAsync(id);
            if (pharmacist == null)
            {
                TempData["Error"] = "الصيدلي غير موجود.";
                return NotFound();
            }

            // تحديث البيانات الأساسية
            pharmacist.UserName = model.UserName;
            pharmacist.Email = model.Email;
            pharmacist.Phone = model.Phone;
            pharmacist.Address = model.Address;
            pharmacist.Shift = model.Shift;
            pharmacist.Permission_Level = model.Permission_Level;
            pharmacist.Salary = model.Salary;
            pharmacist.HireDate = model.HireDate;

            // تحديث كلمة السر إذا تم إدخالها
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    TempData["Error"] = "كلمة المرور وتأكيدها غير متطابقين.";
                    return View(pharmacist);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(pharmacist);
                var result = await _userManager.ResetPasswordAsync(pharmacist, token, NewPassword);

                if (!result.Succeeded)
                {
                    TempData["Error"] = "فشل في تغيير كلمة المرور.";
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(pharmacist);
                }
            }

            try
            {
                var updateResult = await _userManager.UpdateAsync(pharmacist);
                if (updateResult.Succeeded)
                {
                    TempData["Success"] = "تم تعديل بيانات الصيدلي بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "حدث خطأ أثناء حفظ البيانات.";
                    return View(pharmacist);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "حدث تعارض عند تحديث البيانات.";
                return View(pharmacist);
            }
        }


        // ------------------- Delete -------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var emp = await _context.emps.FindAsync(id);
            if (emp == null)
                return NotFound();

            _context.emps.Remove(emp);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Pharmacist deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


        // ------------------- Add Employee (with ViewModel) -------------------
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var identityUser = new emp
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Employee"))
                await _roleManager.CreateAsync(new IdentityRole("Employee"));

            await _userManager.AddToRoleAsync(identityUser, "Employee");

            var empUser = new emp
            {
                Id = identityUser.Id,
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = identityUser.PasswordHash!,
                Permission_Level = "Employee",
                Salary = model.Salary,
                Address = model.Address,
                Phone = model.Phone,
                Shift = model.Shift,
                HireDate = DateTime.Now,
                LoginTimestamp = DateTime.Now
            };

            _context.emps.Add(empUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Employee added and assigned role successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ------------------- Promote To Admin -------------------
        [HttpPost]
        public async Task<IActionResult> PromoteToAdmin(string empId)
        {
            var empUser = await _context.emps.FindAsync(empId);
            if (empUser == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            var identityUser = await _userManager.FindByIdAsync(empId);
            if (identityUser == null)
            {
                TempData["Error"] = "Identity user not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _userManager.IsInRoleAsync(identityUser, "Admin"))
                await _userManager.AddToRoleAsync(identityUser, "Admin");

            if (await _userManager.IsInRoleAsync(identityUser, "Employee"))
                await _userManager.RemoveFromRoleAsync(identityUser, "Employee");

            empUser.Permission_Level = "Admin";

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Promoted to Admin successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Concurrency error occurred during promotion.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // ------------------- Search -------------------
        [HttpGet]
        public IActionResult Search(string searchString)
        {
            ViewBag.SearchString = searchString;

            var employees = from e in _context.emps
                            where e.UserName.Contains(searchString) ||
                                  e.Phone.Contains(searchString)
                            select e;

            if (string.IsNullOrEmpty(searchString))
            {
                employees = _context.emps;
            }

            return View(nameof(Index), employees.ToList());
        }

        // ------------------- Invoices -------------------
        [HttpGet]
        public async Task<IActionResult> Invoices(string id)
        {
            var employee = await _context.emps
                .Include(e => e.Invoices)
                    .ThenInclude(i => i.Items)
                        .ThenInclude(ii => ii.Med)
                .Include(e => e.Invoices)
                    .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            var viewModel = new EmployeeInvoicesViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.UserName,
                Invoices = employee.Invoices.Select(i => new InvoiceViewModel
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    NetTotal = i.NetTotal,
                    PaymentType = i.PaymentType,
                    CustomerName = i.Customer != null ? i.Customer.Name : "Unknown",
                    InvoiceItems = i.Items.Select(ii => new InvoiceItemViewModel
                    {
                        InvoiceItemId = ii.InvoiceItemId,
                        MedicineName = ii.Med != null ? ii.Med.Name : "Unknown",
                        Quantity = ii.Quantity,
                        MRP = (double)ii.MRP,
                        SalePrice = (double)ii.SalePrice,
                        PrushereDate = ii.PrushereDate,
                        DiscountPercentage = ii.DiscountPercentage,
                        Total = ii.Total
                    }).ToList()
                }).OrderByDescending(i => i.InvoiceDate).ToList()
            };

            return View(viewModel);
        }
    }
}