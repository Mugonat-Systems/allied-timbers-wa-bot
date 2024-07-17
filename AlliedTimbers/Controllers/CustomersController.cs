using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;
using PagedList;

namespace AlliedTimbers.Controllers;

[Authorize(Roles = "Admin, Manager")]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public async Task<ActionResult> Index(string search)
    {
        if (string.IsNullOrEmpty(search)) return View();

        var user = await _db.Customers
            .FirstOrDefaultAsync(u => u.Name.Contains(search) || u.PhoneNumber.Contains(search));

        if (user == null) return View();

        return RedirectToAction(nameof(Details), new { user.Id });
    }

    public ActionResult Filter(string id, DateTime? start, DateTime? end)
    {
        ViewBag.State = id ?? "All";

        var startDate = start ?? DateTime.Today;
        var endDate = end ?? DateTime.Now;

        var maxDateBack = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
        var dateBefore = new Func<DateTime?, bool>(d => startDate > d.GetValueOrDefault());
        var dateBetween =
            new Func<DateTime?, bool>(d => startDate <= d.GetValueOrDefault() && endDate >= d.GetValueOrDefault());
        var customers = _db.Customers.ToList();

        return id switch
        {
            "available" => View(customers.Where(a => dateBetween(a.LastAvailableOn)).ToList()),
            "missed" => View(customers.Where(c =>
                c.LastRepliedOn > maxDateBack && dateBefore(c.LastRepliedOn) && c.IsClosed != true).ToList()),
            "blocked" => View(customers.Where(y => y.ChancesLeft <= 0).ToList()),
            _ => View(customers.ToList())
        };
    }

    public ActionResult Details(int? id, int page = 1)
    {
        if (id == null) return RedirectToAction(nameof(Index));

        var user = _db.Customers.Find(id);

        if (user == null) return RedirectToAction(nameof(Index));

        ViewBag.Messages = _db.Messages
            .Where(m => m.ToPhone == user.PhoneNumber || m.FromPhone == user.PhoneNumber)
            .OrderByDescending(m => m.SentTimeStamp)
            .ToPagedList(page, 20);

        return View(user);
    }

    public async Task<ActionResult> Unblock(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var customer = await _db.Customers.FindAsync(id);

        if (customer == null) return HttpNotFound();

        customer.ChancesLeft = 5;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}