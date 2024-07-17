using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlliedTimbers.Utilities;
using System.Security.Authentication;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductInfo
        public async Task<ActionResult> Index()
        {
            return View(await db.ProductInfos.ToListAsync());
        }

        // GET: ProductInfo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductInfo productInfo = await db.ProductInfos
                .SingleOrDefaultAsync(x => x.Id == id);

            if (productInfo == null)
            {
                return HttpNotFound();
            }
            return View(productInfo);
        }



        // GET: ProductInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( ProductInfo productInfo)
        {
            if (productInfo != default)
            {
                //try
                //{
                //    if (productInfo.FilePath != default)
                //    {
                //        productInfo.FilePath = this.Upload(nameof(productInfo.FilePath));
                //        productInfo.Size = FileHandler.GetFileSize(productInfo.FilePath);
                //        productInfo.Type = FileHandler.FileType(productInfo.FilePath);
                //    }
                   
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}
                
                productInfo.IsChecked =false;

                db.ProductInfos.Add(productInfo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(productInfo);
        }

        // GET: ProductInfo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductInfo productInfo = await db.ProductInfos.FindAsync(id);
            if (productInfo == null)
            {
                return HttpNotFound();
            }
            return View(productInfo);
        }

        // POST: ProductInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductInfo productInfo, HttpPostedFileBase filePath)
        {
            if (productInfo != default)
            {
                //if(filePath != null)
                //{
                //    productInfo.FilePath = FileHandler.Upload(filePath);
                //}

                db.Entry(productInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productInfo);
        }

        // GET: ProductInfo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductInfo productInfo = await db.ProductInfos.FindAsync(id);
            if (productInfo == null)
            {
                return HttpNotFound();
            }
            return View(productInfo);
        }

        // POST: ProductInfo/Delete/5
        [Audit]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductInfo productInfo = await db.ProductInfos.FindAsync(id);
            db.ProductInfos.Remove(productInfo);
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
}
