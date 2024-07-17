//using System.Data.Entity;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using AlliedTimbers.Models;
//using AlliedTimbers.Utilities;

//namespace AlliedTimbers.Controllers
//{
//    public class ProductFilesController : Controller
//    {
//        private readonly ApplicationDbContext _db = new();

//        // GET: ProductFiles
//        public async Task<ActionResult> Index()
//        {
//            return View(await _db.ProductFiles.ToListAsync());
//        }


//        // GET: ProductFiles/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: ProductFiles/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create( ProductFile productFile)
//        {
//            if (productFile != default)
//            {
//                if (productFile.Path.Length > 0)
//                    productFile.Path = this.Upload(nameof(productFile.Path));

//                productFile.Size = FileHandler.GetFileSize(productFile.Path);
//                productFile.Type = FileHandler.FileType(productFile.Path);
//                productFile.IsChecked = false;

//                _db.ProductFiles.Add(productFile);
//                await _db.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }

//            return View(productFile);
//        }

//        // GET: ProductFiles/Edit/5
//        public async Task<ActionResult> Edit(int? id)
//        {
//            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            var productFile = await _db.ProductFiles.FindAsync(id);
//            if (productFile == null) return HttpNotFound();

//            return View(productFile);
//        }

//        // POST: ProductFiles/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Edit(ProductFile productFile, HttpPostedFileBase filePath)
//        {
//            if (productFile != default)
//            {
//                if (filePath != null)
//                {
//                    productFile.Path = FileHandler.Upload(filePath);
//                }

//                _db.Entry(productFile).State = EntityState.Modified;
//                await _db.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }

//            return View(productFile);
//        }

//        // GET: ProductFiles/Delete/5
//        public async Task<ActionResult> Delete(int? id)
//        {
//            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            var productFile = await _db.ProductFiles.FindAsync(id);
//            if (productFile == null) return HttpNotFound();
//            return View(productFile);
//        }

//        // POST: ProductFiles/Delete/5
//        [HttpPost]
//        [ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> DeleteConfirmed(int id)
//        {
//            var productFile = await _db.ProductFiles.FindAsync(id);
//            if (productFile != null) _db.ProductFiles.Remove(productFile);
//            await _db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing) _db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}