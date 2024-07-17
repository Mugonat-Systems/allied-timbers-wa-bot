using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

[Authorize(Roles ="Admin")]
public class CompanyBranchesController : Controller
{
    private readonly ApplicationDbContext _db = new();

    // GET: CompanyBranches
    public async Task<ActionResult> Index()
    {
        var companyBranches = _db.CompanyBranches.Include(c => c.Address);
        return View(await companyBranches.ToListAsync());
    }

    

    // GET: CompanyBranches/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: CompanyBranches/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(CompanyBranch companyBranch)
    {
        if (ModelState.IsValid || companyBranch != default)
        {
            CompanyBranch company = new CompanyBranch
            {
                Name = companyBranch.Name,
                Email = companyBranch.Email,
                PhoneNumber = companyBranch.PhoneNumber,
                Address = new Address
                {
                    Name = companyBranch.Address.Name,
                    Line1 = companyBranch.Address.Line1,
                    Line2 = companyBranch.Address.Line2,
                }
            };


            _db.CompanyBranches.Add(company);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(companyBranch);
    }

    // GET: CompanyBranches/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var companyBranch = await _db.CompanyBranches.FindAsync(id);
        if (companyBranch == null) return HttpNotFound();
        ViewBag.Id = new SelectList(_db.Addresses, "Id", "Name", companyBranch.Id);
        return View(companyBranch);
    }

    // POST: CompanyBranches/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Email,PhoneNumber")] CompanyBranch companyBranch)
    {
        if (ModelState.IsValid)
        {
            _db.Entry(companyBranch).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ViewBag.Id = new SelectList(_db.Addresses, "Id", "Name", companyBranch.Id);
        return View(companyBranch);
    }

    // GET: CompanyBranches/Delete/5
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var companyBranch = await _db.CompanyBranches.FindAsync(id);
        if (companyBranch == null) return HttpNotFound();
        
        return View(companyBranch);
    }

    //POST: CompanyBranches/Delete/5
    [Audit]
     [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        CompanyBranch companyBranch = await _db.CompanyBranches.FindAsync(id);
       _db.CompanyBranches.Remove(companyBranch);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}