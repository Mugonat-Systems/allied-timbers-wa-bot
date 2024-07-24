using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    public class EducationalController : Controller
    {
        private readonly ApplicationDbContext _db = new();

        // GET: Educational
        public async Task<ActionResult> Index()
        {
            return View(await _db.Educationals.ToListAsync());
        }

   

        // GET: Educational/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Educational/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,Content,Date")] Educational educationals)
        {
            if (ModelState.IsValid)
            {
                _db.Educationals.Add(educationals);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(educationals);
        }

        // GET: Educational/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var educationals = await _db.Educationals.FindAsync(id);
            if (educationals == null) return HttpNotFound();
            return View(educationals);
        }

        // POST: Educational/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Content,Date")] Educational educationals)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(educationals).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(educationals);
        }

        // GET: Educational/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var educationals = await _db.Educationals.FindAsync(id);
            if (educationals == null) return HttpNotFound();
            return View(educationals);
        }

        // POST: Educational/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var educationals = await _db.Educationals.FindAsync(id);
            if (educationals != null) _db.Educationals.Remove(educationals);
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