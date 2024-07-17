using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class LoanApplicationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LoanApplications
        public async Task<ActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
            {
                try
                {
                    var branch = db.Users.FirstOrDefault(x => x.UserName
                     == User.Identity.Name).Branch;

                    var loanApplications = db.LoanApplications
                       .Where(s => s.BranchName == branch)
                       .OrderByDescending(x => x.DateApplied);
                    return View(await loanApplications.ToListAsync());
                }
                catch { }
            }
            
            var loans = await db.LoanApplications
            .OrderByDescending(x => x.DateApplied).ToListAsync();
            return View(loans);

        }

        // GET: LoanApplications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = await db.LoanApplications
                .SingleOrDefaultAsync(x => x.Id == id);

            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        // GET: LoanApplications/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: LoanApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CustomerName,Amount,IdNo,PhoneNo,LoanApproval,DateApplied,ProductId")] LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {
                db.LoanApplications.Add(loanApplication);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Products, "Id", "Name", loanApplication.Id);
            return View(loanApplication);
        }

        // GET: LoanApplications/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = await db.LoanApplications.FindAsync(id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
           
            return View(loanApplication);
        }

        // POST: LoanApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CustomerName,Amount,IdNo,PhoneNo,BranchName,LoanApproval,DateApplied,ProductName")] LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanApplication).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
 
            return View(loanApplication);
        }

        // GET: LoanApplications/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = await db.LoanApplications.FindAsync(id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        // POST: LoanApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LoanApplication loanApplication = await db.LoanApplications.FindAsync(id);
            db.LoanApplications.Remove(loanApplication);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DeleteRange(IEnumerable<int?> loanIds)
        {
            var loans = await db.LoanApplications.Where(d => 
            loanIds.Contains(d.Id)).ToListAsync();

            foreach(var  loan in loans)
            {
                db.LoanApplications.Remove(loan);
                await db.SaveChangesAsync();
            }

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
