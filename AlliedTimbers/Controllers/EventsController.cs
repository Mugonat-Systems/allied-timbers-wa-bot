using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db = new();

        // GET: Events
        public async Task<ActionResult> Index()
        {
            return View(await _db.Events.ToListAsync());
        }

   

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Venue,StartDate,EndDate,Time")] Event events)
        {
            if (ModelState.IsValid)
            {
                _db.Events.Add(events);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var events = await _db.Events.FindAsync(id);
            if (events == null) return HttpNotFound();
            return View(events);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Venue,StartDate,EndDate,Time")] Event events)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(events).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(events);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var events = await _db.Events.FindAsync(id);
            if (events == null) return HttpNotFound();
            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var events = await _db.Events.FindAsync(id);
            if (events != null) _db.Events.Remove(events);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}