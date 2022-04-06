using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ASMWebTest1Project.Startup))]

namespace ASMWebTest1Project
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Authen/Login")

            });
            CreateUserRole();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
        private void CreateUserRole()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>();
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);

            }
            var user = new IdentityUser("Admin");
            var result = manager.Create(user, "123456");

            if (result.Succeeded)
            {
                manager.AddToRole(user.Id, "Admin");
            }
            if (!roleManager.RoleExists("Qualitity Assurance Manager"))
            {
                var role = new IdentityRole("Qualitity Assurance Manager");
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Quality Assurance Coordinator"))
            {
                var role = new IdentityRole("Quality Assurance Coordinator");
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Staff"))
            {
                var role = new IdentityRole("Staff");
                roleManager.Create(role);
            }
        }
    }
}
