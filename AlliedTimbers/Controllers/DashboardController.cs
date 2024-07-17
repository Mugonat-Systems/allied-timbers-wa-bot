using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

[Authorize(Roles = "Admin, Manager")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public ActionResult Index() => View(new ChatHistoryReport(_db, DateTime.Today, DateTime.Now));

    public ActionResult Historic(DateTime? start, DateTime? end) =>
        View(new ChatHistoryReport(_db, start ?? DateTime.Today, end ?? DateTime.Now));

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _db.Dispose();
        }

        base.Dispose(disposing);
    }
}