using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class SupplierController:BaseController<Supplier>
    {
        public IActionResult Search(string searchString)
        {
            var customers = from c in _context.Suppliers
                            where c.Sup_Name!.Contains(searchString)
                            select c;

            if (string.IsNullOrEmpty(searchString))
            {
                customers = _context.Suppliers;
            }

            ViewBag.SearchString = searchString;
            return View(nameof(Index), customers.ToList());
        }

    }
}
