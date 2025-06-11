using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController<Category>
    {
        // GET: /Category/Medicines/5
        public IActionResult Medicines(int id)
        {
            var medicines = _context.Medicines
                .Where(m => m.CategoryId == id)
                .ToList();
            ViewBag.CategoryName = _context.Categories
                                    .Where(c => c.Cat_ID == id)
                                    .Select(c => c.Cat_Name)
                                    .FirstOrDefaultAsync();



            return View(medicines);
        }

    }
}