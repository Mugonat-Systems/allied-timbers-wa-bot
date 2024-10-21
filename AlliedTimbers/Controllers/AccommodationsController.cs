using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers;

    public class AccommodationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accommodations
        public ActionResult Index()
        {
            return View(db.Accommodations.ToList());
        }

        // GET: Accommodations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accommodation accommodation = db.Accommodations.Find(id);
            if (accommodation == null)
            {
                return HttpNotFound();
            }
            return View(accommodation);
        }

        // GET: Accommodations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accommodations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Image1,Image2,Status")] Accommodation accommodation, System.Web.HttpPostedFileBase imageFile1, System.Web.HttpPostedFileBase imageFile2)
        {
            if (ModelState.IsValid)
            {
            string imagePath1 = null;
            if (imageFile1 != null && imageFile1.ContentLength > 0)
            {
                // Generate a unique file name
                string fileName = Path.GetFileNameWithoutExtension(imageFile1.FileName);
                string extension = Path.GetExtension(imageFile1.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                // Define the path to store the image
                string directory =Server.MapPath("/Images/Accommodations/");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                imagePath1 = Path.Combine(directory, fileName);
                imageFile1.SaveAs(imagePath1);

                // Store the relative path in the database
                imagePath1 = "/Images/Accommodations/" + fileName;
            }

            string imagePath2 = null;
            if (imageFile2 != null && imageFile2.ContentLength > 0)
            {
// Generate a unique file name
                string fileName = Path.GetFileNameWithoutExtension(imageFile2.FileName);
                string extension = Path.GetExtension(imageFile2.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                // Define the path to store the image
                string directory = Server.MapPath("/Images/Accommodations/");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                imagePath2 = Path.Combine(directory, fileName);
                imageFile2.SaveAs(imagePath2);

                // Store the relative path in the database
                imagePath2 = "/Images/Accommodations/" + fileName;
            }

            accommodation.Image1 = imagePath1;
            accommodation.Image2 = imagePath2;
            
            db.Accommodations.Add(accommodation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accommodation);
        }

        // GET: Accommodations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accommodation accommodation = db.Accommodations.Find(id);
            if (accommodation == null)
            {
                return HttpNotFound();
            }
            return View(accommodation);
        }

    // POST: Accommodations/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Image1,Image2,Status")] Accommodation accommodation, System.Web.HttpPostedFileBase imageFile1, System.Web.HttpPostedFileBase imageFile2)
    {
        if (ModelState.IsValid)
        {
            var updateAccommodation = db.Accommodations.Find(accommodation.Id); // Find the existing entity in the database
            if (updateAccommodation != null)
            {
                // Update properties
                updateAccommodation.Name = accommodation.Name;
                updateAccommodation.Description = accommodation.Description;
                updateAccommodation.Price = accommodation.Price;
                updateAccommodation.Status = accommodation.Status;

                // Handle imageFile1 if a new image is uploaded
                if (imageFile1 != null && imageFile1.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageFile1.FileName);
                    string extension = Path.GetExtension(imageFile1.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    string directory = Server.MapPath("/Images/Accommodations/");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string imagePath1 = Path.Combine(directory, fileName);
                    imageFile1.SaveAs(imagePath1);

                    updateAccommodation.Image1 = "/Images/Accommodations/" + fileName; // Update the image path
                }

                // Handle imageFile2 if a new image is uploaded
                if (imageFile2 != null && imageFile2.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageFile2.FileName);
                    string extension = Path.GetExtension(imageFile2.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    string directory = Server.MapPath("/Images/Accommodations/");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string imagePath2 = Path.Combine(directory, fileName);
                    imageFile2.SaveAs(imagePath2);

                    updateAccommodation.Image2 = "/Images/Accommodations/" + fileName; // Update the image path
                }

                // Now save changes to the database
                db.Entry(updateAccommodation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return View(accommodation);
    }


    // GET: Accommodations/Delete/5
    public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accommodation accommodation = db.Accommodations.Find(id);
            if (accommodation == null)
            {
                return HttpNotFound();
            }
            return View(accommodation);
        }

        // POST: Accommodations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Accommodation accommodation = db.Accommodations.Find(id);
            db.Accommodations.Remove(accommodation);
            db.SaveChanges();
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

