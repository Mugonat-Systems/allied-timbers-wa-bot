using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AlliedTimbers.Models;

namespace AlliedTimbers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorsController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AdministratorsController()
        {
        }

        public AdministratorsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        
        // GET: Users by username
        public async Task<ActionResult> Index()
        {
            return View(await _db.Users.AsNoTracking().ToListAsync());
        }

        public ActionResult Create()
        {
            return View();
        }
        
        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email,
                    Branch = model.BranchName, RoleName = model.UserRole};
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));


                var result = await UserManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                 
                    roleManager.Create(new IdentityRole(model.UserRole));
                    UserManager.AddToRole(user.Id, model.UserRole);
                    
                    //await SignInManager.SignInAsync(user, false, false);

                    return RedirectToAction("Index");

                }
                AddErrors(result);
            }

                           
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = _db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return View(applicationUser);
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid) return View(applicationUser);

            var user = UserManager.FindById(applicationUser.Id);
            if (user == null)
            {
                return View(applicationUser);
            }

            user.Branch = applicationUser.Branch;
            user.UserName = applicationUser.Email;
            user.Email = applicationUser.Email;
            user.RoleName = applicationUser.RoleName;

            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
                await UserManager.UpdateAsync(user);

                var role = await roleManager.FindByNameAsync(applicationUser.RoleName);
                role.Name = applicationUser.RoleName;
                await roleManager.UpdateAsync(role);

                return RedirectToAction("Index");
            }

            catch (Exception e)
            {
                Logger.Log(e.Message);
            }


            return View(applicationUser);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = _db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return View(applicationUser);
        }

        [Audit]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var applicationUser = await UserManager.FindByIdAsync(id);
                UserManager.RemoveFromRole(applicationUser.Id, applicationUser.RoleName);
                var result = await UserManager.DeleteAsync(applicationUser);
                if (result.Succeeded)
                    return RedirectToAction("Index");

            }
            catch (Exception ) { }

             return RedirectToAction("Index"); ;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                
                if (disposing) _db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}