using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _db = new();

        // GET: Promotions
        public async Task<ActionResult> Index()
        {
            return View(await _db.Promotions.ToListAsync());
        }

   

        // GET: Promotions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _db.Promotions.Add(promotion);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var promotion = await _db.Promotions.FindAsync(id);
            if (promotion == null) return HttpNotFound();
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(promotion).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var promotion = await _db.Promotions.FindAsync(id);
            if (promotion == null) return HttpNotFound();
            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _db.Promotions.FindAsync(id);
            if (promotion != null) _db.Promotions.Remove(promotion);
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