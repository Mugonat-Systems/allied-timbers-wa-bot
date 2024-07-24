using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db = new();

        // GET: News
        public async Task<ActionResult> Index()
        {
            return View(await _db.News.ToListAsync());
        }

   

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,Date")] News news)
        {
            if (ModelState.IsValid)
            {
                _db.News.Add(news);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var news = await _db.News.FindAsync(id);
            if (news == null) return HttpNotFound();
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Date")] News news)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(news).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var news = await _db.News.FindAsync(id);
            if (news == null) return HttpNotFound();
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news != null) _db.News.Remove(news);
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