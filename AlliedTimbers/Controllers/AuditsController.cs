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
    public class AuditsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Audits
        public async Task<ActionResult> Index()
        {
            return View(await db.Audits.ToListAsync());
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

    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
             

            Audit audit = new Audit
            {
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name :
                "Anonymous",
                AreaAccessed = request.RawUrl,
                Timestamp = DateTime.Now,
                Action = filterContext.ActionDescriptor.ActionName,
            };


            ApplicationDbContext db = new ApplicationDbContext();
            db.Audits.Add(audit);
            db.SaveChanges();
            base.OnActionExecuting(filterContext);
        }
    }
}
