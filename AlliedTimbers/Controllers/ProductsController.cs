using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Web.Management;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db = new();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await _db.Products.Include(s => s.Files)
                .Include(w => w.Information)
                .OrderBy(s => s.Name)
                .ToListAsync());
        }


        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                //List<ProductFile> files = new();
                List<ProductInfo> infos = new();
                //try
                //{
                //    var productFiles = await _db.ProductFiles.ToListAsync();    
                //    foreach (var f in productFiles)
                //    {
                //        if (productViewModel.FileIds.Contains(f.Id))
                //        {
                //            f.IsChecked = true;
                //            files.Add(f);
                //        }
                //    }
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}

                try
                {
                    var productInfos = await _db.ProductInfos.ToListAsync();
                    foreach (var productInfo in productInfos)
                    {
                        if (productViewModel.InfoIds.Contains(productInfo.Id))
                        {
                            productInfo.IsChecked = true;
                            infos.Add(productInfo);
                        }
                    }
                }
                catch (Exception)
                {
                }
            
                Product product = new Product
                {
                    Name = productViewModel.Name,
                    Description = productViewModel.Description,
                    Requirements = productViewModel.Requirements,
                    IsLoan = productViewModel.IsLoan,
                    IsImageRequired = productViewModel.IsImageRequired,
                    IsMukando = productViewModel.IsMukando,
                    IsSolar = productViewModel.IsSolar,
                    IsBoards = productViewModel.IsBoards,
                    IsPoles = productViewModel.IsPoles,
                    IsTrusses = productViewModel.IsTrusses,
                    IsDoors = productViewModel.IsDoors,
                    IsTimber = productViewModel.IsTimber,
                   // Files = files,
                    Information = infos
                };

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = await _db.Products.FindAsync(id);

            if (product == null) return HttpNotFound();

           // var productFiles = await _db.ProductFiles.ToListAsync();
            var productInfo = await _db.ProductInfos.ToListAsync();

            //var files = productFiles.Except(product.Files).ToList();
            var info = productInfo.Except(product.Information).ToList();
       
           // ViewBag.Files = files;
            ViewBag.Info = info;


            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Product product)
        {
            if (ModelState.IsValid)
            {
                //List<ProductFile> files = new();
                List<ProductInfo> infos = new();

                try
                {
                    var productInfos = await _db.ProductInfos.ToListAsync();
                    foreach (var productInfo in productInfos)
                    {
                        if (product.InfoIds.Contains(productInfo.Id))
                        {
                            infos.Add(productInfo);
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
                //try
                //{
                //    var productFiles = await _db.ProductFiles.ToListAsync();    
                //    foreach (var f in productFiles)
                //    {
                //        if (product.FileIds.Contains(f.Id))
                //        {
                //            files.Add(f);
                //        }
                //    }
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}

                var updateProduct = await _db.Products.FindAsync(product.Id);
                if (updateProduct != null)
                {
                    updateProduct.Name = product.Name;
                    updateProduct.Description = product.Description;
                    updateProduct.Requirements = product.Requirements;
                    updateProduct.Information = infos;
                    updateProduct.IsLoan = product.IsLoan;
                    updateProduct.IsImageRequired = product.IsImageRequired;
                    updateProduct.IsMukando = product.IsMukando;
                    updateProduct.IsSolar = product.IsSolar;
                    updateProduct.IsBoards = product.IsBoards;
                    updateProduct.IsPoles = product.IsPoles;
                    updateProduct.IsTrusses = product.IsTrusses;
                    updateProduct.IsDoors = product.IsDoors;
                    updateProduct.IsTimber = product.IsTimber;
                    //updateProduct.Files = files;
                }

                _db.Products.AddOrUpdate(updateProduct);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Delete/5
        [Audit]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = await _db.Products.FindAsync(id);
            if (product == null) return HttpNotFound();

            if (product != default)
            {
               // var productFile = await _db.ProductFiles.Where(x => x.Product.Id == id).ToListAsync();
                var productInfo = await _db.ProductInfos.Where(x => x.Product.Id == id).ToListAsync();
               
                //  _db.ProductFiles.RemoveRange(productFile);
               
                _db.ProductInfos.RemoveRange(productInfo);
                await _db.SaveChangesAsync();

                _db.Products.Remove(product);

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteRange(IEnumerable<int?> prodIds)
        //{
        //    var productInfo = await _db.ProductInfos.Where(
        //        x => prodIds.Contains(x.ProductId))
        //        .ToListAsync();
         
        //    if(productInfo.Count != 0 )
        //    {
        //        foreach (var info in productInfo)
        //        {
        //            _db.ProductInfos.Remove(info);
        //            await _db.SaveChangesAsync();
        //        }
        //    }
           

        //    var products = await _db.Products.Where(s =>
        //    prodIds.Contains(s.Id)).ToListAsync();

        //    foreach (var product in products)
        //    {
        //        _db.Products.Remove(product);
        //        await _db.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Index");
        //}


        // POST: Products/Delete/5
        //[HttpPost]
        //[ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _db.Products.FindAsync(id);
        //    var productFile = await _db.ProductFiles.Where(x => x.Product.Id == id).ToListAsync();
        //    var productInfo = await _db.ProductInfos.Where(x => x.Product.Id == id).ToListAsync();

        //    _db.ProductFiles.RemoveRange(productFile);
        //    _db.ProductInfos.RemoveRange(productInfo);
        //    _db.Products.Remove(product);


        //    await _db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}






        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}