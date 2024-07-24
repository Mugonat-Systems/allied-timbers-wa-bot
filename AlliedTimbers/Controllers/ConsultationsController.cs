using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

[Authorize(Roles ="Admin")]
public class ConsultationsController : Controller
{
    private readonly ApplicationDbContext _db = new();

    // GET: Consultation
    public async Task<ActionResult> Index()
    {
            return View(await _db.Consultations.ToListAsync());
    }

    

    // GET: Consultation/Create
    public ActionResult Create()
    {
        return View();
    }


    // GET: Consultation/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var consultation = await _db.Consultations.FindAsync(id);
        if (consultation == null) return HttpNotFound();
        ViewBag.Id = new SelectList(_db.Appointments, "Id", "Name", consultation.Id);
        return View(consultation);
    }

    // POST: Appointment/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Email,Date,Time,Phone,ServiceType,ConsultationStatus")] Consultation consultation)
    {
        if (ModelState.IsValid)
        {
            _db.Entry(consultation).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ViewBag.Id = new SelectList(_db.Consultations, "Id", "Name", consultation.Id);
        return View(consultation);
    }

    // GET: Consultation/Delete/5
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var consultation = await _db.Consultations.FindAsync(id);
        if (consultation == null) return HttpNotFound();
        
        return View(consultation);
    }

    //POST: Consultation/Delete/5
    [Audit]
     [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        Consultation consultation = await _db.Consultations.FindAsync(id);
       _db.Consultations.Remove(consultation);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}