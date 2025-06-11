using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<emp> _userManager;

        public ProfileController(AppDbContext context, UserManager<emp> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Signin", "Account");

            var employee = await _userManager.Users
                .OfType<emp>()
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (employee == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                User_Id = employee.Id,
                username = employee.UserName,
                email = employee.Email,
                Password_Hash = employee.PasswordHash,
                Permission_Level = employee.Permission_Level,
                Salary = employee.Salary??0,
                Address = employee.Address,
                Phone = employee.Phone,
                Shift = employee.Shift,
                HireDate = employee.HireDate,
                LoginTimestamp = employee.LoginTimestamp
            };

            return View(model);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProfileViewModel
            {
                HireDate = DateTime.Now, // Default to today's date: 03:25 AM EEST on Monday, June 09, 2025
                LoginTimestamp = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = new emp
            {
                Id = model.User_Id,
                UserName = model.username,
                Email = model.email,
                PasswordHash = model.Password_Hash,
                Permission_Level = model.Permission_Level,
                Salary = model.Salary,
                Address = model.Address,
                Phone = model.Phone,
                Shift = model.Shift,
                HireDate = model.HireDate,
                LoginTimestamp = model.LoginTimestamp
            };

            _context.emps.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var employee = await _context.emps
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return NotFound();

            var model = new ProfileViewModel
            {
                User_Id = employee.Id,
                username = employee.UserName,
                email = employee.Email,
                Password_Hash = employee.PasswordHash,
                Permission_Level = employee.Permission_Level,
                Salary = employee.Salary?? 0,
                Address = employee.Address,
                Phone = employee.Phone,
                Shift = employee.Shift,
                HireDate = employee.HireDate,
                LoginTimestamp = employee.LoginTimestamp
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ProfileViewModel model)
        {
            if (id != model.User_Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await _context.emps.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            employee.UserName = model.username;
            employee.Email = model.email;
            employee.PasswordHash = model.Password_Hash;
            employee.Permission_Level = model.Permission_Level;
            employee.Salary = model.Salary;
            employee.Address = model.Address;
            employee.Phone = model.Phone;
            employee.Shift = model.Shift;
            employee.HireDate = model.HireDate;
            employee.LoginTimestamp = model.LoginTimestamp;

            await _context.SaveChangesAsync();

            // ✅ إعادة التوجيه إلى صفحة Index بعد نجاح التعديل
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var employee = await _context.emps
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return NotFound();

            var model = new ProfileViewModel
            {
                User_Id = employee.Id,
                username = employee.UserName,
                email = employee.Email,
                Password_Hash = employee.PasswordHash,
                Permission_Level = employee.Permission_Level,
                Salary = employee.Salary ?? 0,
                Address = employee.Address,
                Phone = employee.Phone,
                Shift = employee.Shift,
                HireDate = employee.HireDate,
                LoginTimestamp = employee.LoginTimestamp
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employee = await _context.emps.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            _context.emps.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}