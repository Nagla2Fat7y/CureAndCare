using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;

namespace Pharmacy.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<emp> _userManager;
        private readonly SignInManager<emp> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<emp> userManager,
            SignInManager<emp> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        #region SignUp

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userByName = await _userManager.FindByNameAsync(model.Username);
            var userByEmail = await _userManager.FindByEmailAsync(model.Email);

            if (userByName != null)
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            if (userByEmail != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            var user = new emp
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true,

                // بيانات إضافية خاصة بالموظف
                Permission_Level = "Employee",
                Salary = 3000,
                Address = "Fayoum",
                Phone = "0112255707",
                Shift = "Night",
                HireDate = DateTime.Now,
                LoginTimestamp = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // تأكد من وجود الرول "Employee"
                if (!await _roleManager.RoleExistsAsync("Employee"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Employee"));
                }

                // أضف المستخدم إلى الرول "Employee"
                await _userManager.AddToRoleAsync(user, "Employee");

                return RedirectToAction(nameof(SignIn));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion


        #region Login 

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var User = await _userManager.FindByEmailAsync(model.Email);
            if (User != null)
            {
                var flag = await _userManager.CheckPasswordAsync(User, model.Password);
                if (flag)
                {
                    var res = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false);
                    if (res.Succeeded)
                        return RedirectToAction("Index", "Dashboard");
                }
            }
            ModelState.AddModelError("", "Invaild Login !!!");
            return View(model);
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        #endregion
    }
}
