using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;

public class BaseController<TEntity> : Controller where TEntity : class
{
    protected readonly AppDbContext _context;

    public BaseController()
    {
        _context = new AppDbContext();
    }

    // Index
    public virtual IActionResult Index()
    {
        var items = _context.Set<TEntity>().ToList();
        return View(items);
    }

    // Create - GET
    public virtual IActionResult Create()
    {
        return View();
    }

    // Create - POST
    [HttpPost]
    public virtual IActionResult Create(TEntity entity)
    {
        if (ModelState.IsValid)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    // Edit - GET
    public virtual IActionResult Edit(int id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    // Edit - POST
    [HttpPost]
    public virtual IActionResult Edit(TEntity entity)
    {
        if (ModelState.IsValid)
        {
            _context.Update(entity);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    // Delete
    public virtual IActionResult Delete(int id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null) return NotFound();

        _context.Remove(entity);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
