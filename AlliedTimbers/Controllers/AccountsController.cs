using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

public class AccountsController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Accounts
    public async Task<ActionResult> Index()
    {
        var accounts = await db.Accounts.ToListAsync();

        return View(accounts);
    }


    // GET: Accounts/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Accounts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Name,Surname,Email,Region")] Account account)
    {
        if (ModelState.IsValid)
        {
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(account);
    }

    // GET: Accounts/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Account account= await db.Accounts.FindAsync(id);
        if (account == null)
        {
            return HttpNotFound();
        }
        return View(account);
    }

    // POST: Accounts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Name,Surname,Email,Region")] Account
        account)
    {
        if (ModelState.IsValid)
        {
            db.Entry(account).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(account);
    }

    // GET: Accounts/Details/5
    public async Task<ActionResult> Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Account account = await db.Accounts
            .SingleOrDefaultAsync(x => x.Id == id);

        if (account == null)
        {
            return HttpNotFound();
        }
        return View(account);
    }

    // GET: Accounts/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Account account = await db.Accounts.FindAsync(id);
        if (account == null)
        {
            return HttpNotFound();
        }
        return View(account);
    }

    // POST: Accounts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        Account account = await db.Accounts.FindAsync(id);
        db.Accounts.Remove(account);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}