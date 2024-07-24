using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

[Authorize(Roles ="Admin")]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _db = new();

    // GET: Appointment
    public async Task<ActionResult> Index()
    {
            return View(await _db.Appointments.ToListAsync());
    }

    

    // GET: Appointment/Create
    public ActionResult Create()
    {
        return View();
    }


    // GET: Appointment/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment == null) return HttpNotFound();
        ViewBag.Id = new SelectList(_db.Appointments, "Id", "Name", appointment.Id);
        return View(appointment);
    }

    // POST: Appointment/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [Audit]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Email,Date,Time,Phone,ServiceType,AppointmentStatus")] Appointment appointment)
    {
        if (ModelState.IsValid)
        {
            _db.Entry(appointment).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ViewBag.Id = new SelectList(_db.Appointments, "Id", "Name", appointment.Id);
        return View(appointment);
    }

    // GET: Appointment/Delete/5
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment == null) return HttpNotFound();
        
        return View(appointment);
    }

    //POST: Appointment/Delete/5
    [Audit]
     [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        Appointment appointment = await _db.Appointments.FindAsync(id);
       _db.Appointments.Remove(appointment);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}