using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ASMWebTest1Project.Models;
using Microsoft.AspNet.Identity;

namespace ASMWebTest1Project.Controllers
{
    [Authorize]
    public class InteractivesController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: Interactives
        public ActionResult Index()
        {
            var interactive = db.Interactive.Include(i => i.Ideas);
            return View(interactive.ToList());
        }

        // GET: Interactives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactive interactive = db.Interactive.Find(id);
            if (interactive == null)
            {
                return HttpNotFound();
            }
            return View(interactive);
        }
        [Authorize]
        /*public ActionResult Create(int ideaID, int informationID, DateTime createOn ,string comment, string Cname)
        {
            
            CommentBox.Add(ideaID, informationID, DateTime.Now,comment, User.Identity.GetUserName()); 
            return RedirectToAction("Details", "Ideas", new { id = ideaID});
        }*/

        // GET: Interactives/Create
        public ActionResult Create()
        {
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle");
            
            return View();
        }

        // POST: Interactives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "interactiveID,createOn,comment,mode,ideaID, Cname, incognito")] Interactive interactive)
        {
            if (ModelState.IsValid)
            {
                var users = db.Ideas.Find(interactive.ideaID).Iname;
                var user = db.Information.FirstOrDefault(x => x.name == users).email;
                string subject = "Notifaction";
                string body = interactive.Cname + " just comment the idea. Link:" + "https://localhost:44345/Ideas/Details/" + interactive.ideaID;
                MailMessage mm = new MailMessage();
                mm.To.Add(new MailAddress(user));
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
                db.Interactive.Add(interactive);
                db.SaveChanges();
  
            }

           /* ViewBag.ideaID = new SelectList(db.Ideas, interactive.createOn.ToString(), interactive.mode, interactive.ideaID.ToString(), interactive.Cname);*/
            
            return RedirectToAction("Details", "Ideas", new { id = interactive.ideaID });
        }

        

        // GET: Interactives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactive interactive = db.Interactive.Find(id);
            if (interactive == null)
            {
                return HttpNotFound();
            }
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle", interactive.ideaID);
            
            return View(interactive);
        }

        // POST: Interactives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "interactiveID,createOn,comment,mode,ideaID")] Interactive interactive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interactive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle", interactive.ideaID);
            
            return View(interactive);
        }

        // GET: Interactives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactive interactive = db.Interactive.Find(id);
            if (interactive == null)
            {
                return HttpNotFound();
            }
            return View(interactive);
        }

        // POST: Interactives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Interactive interactive = db.Interactive.Find(id);
            db.Interactive.Remove(interactive);
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
}
