using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

[Authorize (Roles = "Admin")]
public class FaqsController : Controller
{
    private readonly ApplicationDbContext _db = new();

    // GET: Faqs
    public async Task<ActionResult> Index()
    {
        return View(await _db.Faqs.ToListAsync());
    }

    // GET: Faqs/Details/5
    public async Task<ActionResult> Details(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var faq = await _db.Faqs.FindAsync(id);
        if (faq == null) return HttpNotFound();
        return View(faq);
    }

    // GET: Faqs/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Faqs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Question,Answer,Tags,CreatedAt,UpdatedAt")] Faq faq)
    {
        if (ModelState.IsValid)
        {
            _db.Faqs.Add(faq);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(faq);
    }

    // GET: Faqs/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var faq = await _db.Faqs.FindAsync(id);
        if (faq == null) return HttpNotFound();
        return View(faq);
    }

    // POST: Faqs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Question,Answer,Tags,CreatedAt,UpdatedAt")] Faq faq)
    {
        if (ModelState.IsValid)
        {
            _db.Entry(faq).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(faq);
    }

    // GET: Faqs/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var faq = await _db.Faqs.FindAsync(id);
        if (faq == null) return HttpNotFound();
        return View(faq);
    }

    // POST: Faqs/Delete/5
    [Audit]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        var faq = await _db.Faqs.FindAsync(id);
        if (faq != null) _db.Faqs.Remove(faq);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}