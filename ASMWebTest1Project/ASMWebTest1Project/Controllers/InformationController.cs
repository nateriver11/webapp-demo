using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASMWebTest1Project.Models;
using PagedList;

namespace ASMWebTest1Project.Controllers
{
    [Authorize]
    public class InformationController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: Information
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Information" : "";
            ViewBag.SortingDOB = String.IsNullOrEmpty(Sorting_Order) ? "DOB_Information" : "DOB_Desc";
            ViewBag.SortingRoll = String.IsNullOrEmpty(Sorting_Order) ? "Roll_Information" : "Roll_Desc";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;

            var informartion = from inf in db.Information select inf;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                informartion = informartion.Where(inf => inf.name.ToUpper().Contains(Search_Data.ToUpper()));
            }
            switch (Sorting_Order)
            {
                case "Name_Information":
                    informartion = informartion.OrderByDescending(inf => inf.name);
                    break;
                case "DOB_Information":
                    informartion = informartion.OrderBy(inf => inf.DOB);
                    break;
                case "DOB_Desc":
                    informartion = informartion.OrderByDescending(inf => inf.DOB);
                    break;
                case "Roll_Information":
                    informartion = informartion.OrderBy(inf => inf.Irole);
                    break;
                case "Roll_Desc":
                    informartion = informartion.OrderByDescending(inf => inf.Irole);
                    break;
                default:
                    informartion = informartion.OrderBy(inf => inf.name);
                    break;
            }
            int Size_Of_Page = 5;
            int No_Of_Page = (Page_No ?? 1);
            return View(informartion.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        // GET: Information/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = db.Information.Find(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            return View(information);
        }

        // GET: Information/Create
        public ActionResult Create()
        {
            ViewBag.departmentID = new SelectList(db.Departments, "departmentID", "departmenName");
            return View();
        }

        // POST: Information/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "informationID,name,DOB,Irole,phoneNumber,email,address, departmentID")] Information information)
        {
            if (ModelState.IsValid)
            {
                db.Information.Add(information);
                db.SaveChanges();
                AuthenController.CreateAccount(information.name, "123456", information.Irole, information.email, information.departmentID);
                return RedirectToAction("Index");
            }

            return View(information);
        }
        public JsonResult CheckUser(string name)
        {
            Information information = new Information();

            using (var context = new ASMWebTest1Entities())
            {
                information = context.Information.Where(a => a.name.ToLower() == name.ToLower()).FirstOrDefault();
            }


            bool status;
            if (information != null)
            {
                //Already registered  
                status = false;
            }
            else
            {
                //Available to use  
                status = true;
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CheckEmail(string email)
        {
            Information information = new Information();

            using (var context = new ASMWebTest1Entities())
            {
                information = context.Information.Where(a => a.email.ToLower() == email.ToLower()).FirstOrDefault();
            }


            bool status;
            if (information != null)
            {
                //Already registered  
                status = false;
            }
            else
            {
                //Available to use  
                status = true;
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }

        // GET: Information/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = db.Information.Find(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            return View(information);
        }

        // POST: Information/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "informationID,name,DOB,Irole,phoneNumber,email,address")] Information information)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(information);
        }

        // GET: Information/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = db.Information.Find(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            return View(information);
        }
        
        // POST: Information/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Information information = db.Information.Find(id);
            db.Information.Remove(information);
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
