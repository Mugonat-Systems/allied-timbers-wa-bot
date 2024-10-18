using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Reflection;
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
        public async Task<ActionResult> Create( ProductViewModel productViewModel, System.Web.HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
             
                List<ProductInfo> infos = new();

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

                // Handle Image File Upload
                string imagePath = null;
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Generate a unique filename
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;

                    // Define the path to store the image
                    string directory= Server.MapPath("~/Images/Products/");

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    imagePath = Path.Combine(directory, fileName);

                    // Save the file
                    imageFile.SaveAs(imagePath);

                    // Store the relative path in the database
                    imagePath = "/Images/Products/" + fileName;



                }

            
                Product product = new Product
                {
                    Name = productViewModel.Name,
                    Description = productViewModel.Description,
                    Image = imagePath,
                    Price = productViewModel.Price,
                    IsBoards = productViewModel.IsBoards,
                    IsPoles = productViewModel.IsPoles,
                    IsTrusses = productViewModel.IsTrusses,
                    IsDoors = productViewModel.IsDoors,
                    IsTimber = productViewModel.IsTimber,
                    Information = infos
                };

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }
        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await _db.Products
                .SingleOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = await _db.Products.FindAsync(id);

            if (product == null) return HttpNotFound();


            var productInfo = await _db.ProductInfos.ToListAsync();


            var info = productInfo.Except(product.Information).ToList();
       
     
            ViewBag.Info = info;


            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Product product, System.Web.HttpPostedFileBase imageFile)
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
                    
                }

                var updateProduct = await _db.Products.FindAsync(product.Id);
                if (updateProduct != null)
                {
                    updateProduct.Name = product.Name;
                    updateProduct.Description = product.Description;
                    updateProduct.Price = product.Price;
                    updateProduct.Information = infos;
                    updateProduct.IsBoards = product.IsBoards;
                    updateProduct.IsPoles = product.IsPoles;
                    updateProduct.IsTrusses = product.IsTrusses;
                    updateProduct.IsDoors = product.IsDoors;
                    updateProduct.IsTimber = product.IsTimber;

                    // Handle Image File Upload (if a new image is uploaded)
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Generate a unique filename
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;

                        // Define the path to store the image
                        string directory = Server.MapPath("~/Images/Products/");

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        string imagePath = Path.Combine(directory, fileName);

                        // Save the new file
                        imageFile.SaveAs(imagePath);

                        // Update the image path in the database
                        updateProduct.Image = "/Images/Products/" + fileName;
                    }
                }

                // Save the changes_db.Products.AddOrUpdate(updateProduct);
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

                var productInfo = await _db.ProductInfos.Where(x => x.Product.Id == id).ToListAsync();

               
                _db.ProductInfos.RemoveRange(productInfo);
                await _db.SaveChangesAsync();

                _db.Products.Remove(product);

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}