using ASMWebTest1Project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASMWebTest1Project.Controllers
{
    public class AuthenController : Controller
    {
        
        // GET: Authen
        public static void CreateAccount(string userName, string password, string role, string email, int? departmentId)
        {
            ASMWebTest1Entities db = new ASMWebTest1Entities();
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            var user = new IdentityUser(userName);
            manager.Create(user, password);

            manager.AddToRole(user.Id, role);
            manager.SetEmail(user.Id, email);

            AspNetUsers update = db.AspNetUsers.ToList().Find(u => u.Id == user.Id);
            update.departmentID = departmentId;
            db.SaveChanges();
        }

        // GET: Authen
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Account acc)
        {
            if (ModelState.IsValid)
            {
                if (acc.Password.Equals(acc.
                    Password))
                {
                    var userStore = new UserStore<IdentityUser>();
                    var manager = new UserManager<IdentityUser>(userStore);

                    var user = new IdentityUser(acc.UserName);
                    var result = manager.Create(user, acc.Password);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Erorr Adding New User");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password not match");
                    }
                }
            }
            return View(acc);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(Account acc)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            var user = manager.Find(acc.UserName, acc.Password);

            if (user != null)
            {
                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                
                authenticationManager.SignIn(new AuthenticationProperties { }, userIdentity);
                return RedirectToAction("Index", "Home");
            }
            return View(acc);
        }

        public ActionResult Logout()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Login", "Authen");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);

                var users = User.Identity.GetUserId();

                if (users != null)
                {
                    
                    model.Id = users;
                    IdentityResult result = manager.ChangePassword(model.Id, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (string error in result.Errors)
                        ModelState.AddModelError("", error);

                    return View(model);
                }


                return HttpNotFound();
            }

            return View(model);
        }

    }
}