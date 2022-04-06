using ASMWebTest1Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace ASMWebTest1Project.Controllers
{
    public class SendMailController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();
        // GET: SendMail
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Send(Email em)
        {
            var user = (from u in db.Information where u.Irole == "Quality Assurance Coordinator" select u.email).ToArray();

            for(int h = 0; h < user.Length; h++)
            {
                string subject = em.Subject;
                string body = em.Body;
                MailMessage mm = new MailMessage();
                mm.To.Add(new MailAddress(user[h]));
                mm.Subject = subject;
                mm.Body = body;
                mm.From = new MailAddress("visualnotifaction@gmail.com");
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("visualnotifaction@gmail.com", "1900100Co");
                smtp.Send(mm);
                
            }
            
            return View();
        }
    }
}