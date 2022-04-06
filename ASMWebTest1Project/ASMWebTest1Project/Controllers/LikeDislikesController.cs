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
    [Authorize]
    public class LikeDislikesController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: LikeDislikes
        public ActionResult Index()
        {
            var likeDislike = db.LikeDislike.Include(l => l.Ideas);
            return View(likeDislike.ToList());
        }

        // GET: LikeDislikes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LikeDislike likeDislike = db.LikeDislike.Find(id);
            if (likeDislike == null)
            {
                return HttpNotFound();
            }
            return View(likeDislike);
        }

        // GET: LikeDislikes/Create
        public ActionResult Create()
        {
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle");
            return View();
        }

        // POST: LikeDislikes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "likeDislikeID,ideaID,IsAction,UserAction")] LikeDislike likeDislike)
        {
            if (ModelState.IsValid)
            {
                Ideas update = db.Ideas.ToList().Find(u => u.ideaID == likeDislike.ideaID);
                if(likeDislike.IsAction == "true")
                {
                    update.likes += 1;
                }else if(likeDislike.IsAction == "false")
                {
                    update.dislikes += 1;
                }
                
                db.LikeDislike.Add(likeDislike);
                db.SaveChanges();
                
            }

            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle", likeDislike.ideaID);
            return RedirectToAction("Details", "Ideas", new { id = likeDislike.ideaID });
        }

        // GET: LikeDislikes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LikeDislike likeDislike = db.LikeDislike.Find(id);
            if (likeDislike == null)
            {
                return HttpNotFound();
            }
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle", likeDislike.ideaID);
            return View(likeDislike);
        }

        // POST: LikeDislikes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "likeDislikeID,ideaID,IsAction,UserAction")] LikeDislike likeDislike)
        {
            if (ModelState.IsValid)
            {
                db.Entry(likeDislike).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ideaID = new SelectList(db.Ideas, "ideaID", "ideaTitle", likeDislike.ideaID);
            return View(likeDislike);
        }

        // GET: LikeDislikes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LikeDislike likeDislike = db.LikeDislike.Find(id);
            if (likeDislike == null)
            {
                return HttpNotFound();
            }
            return View(likeDislike);
        }

        // POST: LikeDislikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LikeDislike likeDislike = db.LikeDislike.Find(id);
            db.LikeDislike.Remove(likeDislike);
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
