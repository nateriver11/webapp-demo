using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASMWebTest1Project.Models;

namespace ASMWebTest1Project.Controllers
{
    public class RepliesController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: Replies
        public ActionResult Index()
        {
            var replies = db.Replies.Include(r => r.Interactive);
            return View(replies.ToList());
        }

        // GET: Replies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Replies replies = db.Replies.Find(id);
            if (replies == null)
            {
                return HttpNotFound();
            }
            return View(replies);
        }

        // GET: Replies/Create
        public ActionResult Create()
        {
            
            
            return View();
        }

        // POST: Replies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "repliesID,comment,interactiveID,createOn,Rname, ideaID")] Replies replies)
        {
            if (ModelState.IsValid)
            {
                db.Replies.Add(replies);
                db.SaveChanges();
                return RedirectToAction("Details", "Ideas", new { id = replies.ideaID });
            }

            
            
            return View(replies);
        }

        // GET: Replies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Replies replies = db.Replies.Find(id);
            if (replies == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.interactiveID = new SelectList(db.Interactive, "interactiveID", "comment", replies.interactiveID);
            return View(replies);
        }

        // POST: Replies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "repliesID,comment,interactiveID,createOn,Rname")] Replies replies)
        {
            if (ModelState.IsValid)
            {
                db.Entry(replies).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.interactiveID = new SelectList(db.Interactive, "interactiveID", "comment", replies.interactiveID);
            return View(replies);
        }

        // GET: Replies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Replies replies = db.Replies.Find(id);
            if (replies == null)
            {
                return HttpNotFound();
            }
            return View(replies);
        }

        // POST: Replies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Replies replies = db.Replies.Find(id);
            db.Replies.Remove(replies);
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
