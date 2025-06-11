using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewModels;

public class CustomerController : BaseController<Customer>
{
    [Authorize]
    public IActionResult Search(string searchString)
    {
        var customers = from c in _context.Customers
                        where c.Name!.Contains(searchString)
                        select c;

        if (string.IsNullOrEmpty(searchString))
        {
            customers = _context.Customers;
        }

        ViewBag.SearchString = searchString;
        return View(nameof(Index), customers.ToList());
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Purchases(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Invoices)
                .ThenInclude(i => i.Items)
                    .ThenInclude(ii => ii.Med)
            .FirstOrDefaultAsync(c => c.Customer_Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        var viewModel = new CustomerPurchasesViewModel
        {
            CustomerId = customer.Customer_Id,
            CustomerName = customer.Name,
            ContactNumber = customer.Phone,
            Address = customer.Add,
            Invoices = customer.Invoices.Select(i => new InvoiceViewModel
            {
                InvoiceId = i.InvoiceId,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                NetTotal = i.NetTotal,
                PaymentType = i.PaymentType,
                InvoiceItems = i.Items.Select(ii => new InvoiceItemViewModel
                {
                    InvoiceItemId = ii.InvoiceItemId,
                    MedicineName = ii.Med != null ? ii.Med.Name : "Unknown",
                    MedId = ii.MedId,
                    PrushereDate = ii.PrushereDate,
                    Quantity = ii.Quantity,
                    MRP =(double) ii.MRP,
                    SalePrice = (double)ii.SalePrice,
                    DiscountPercentage = ii.DiscountPercentage,
                    Total = ii.Total
                }).ToList()
            }).OrderByDescending(i => i.InvoiceDate).ToList()
        };

        return View(viewModel);
    }
}
