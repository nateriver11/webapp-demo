using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ASMWebTest1Project.Models;
using PagedList;

namespace ASMWebTest1Project.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: Categories
        public CategoriesController()
        {
            var category = from cate in db.Categories select cate;
            foreach (var item in category)
            {
                if (item.closureDate > DateTime.Now)
                {
                    item.Cstatus = "Active";
                }
                else if (item.closureDate < DateTime.Now)
                {
                    item.Cstatus = "Not Active";
                }
                if (item.finalDate < DateTime.Now)
                {
                    item.Cstatus = "Finish";
                }
            }
            db.SaveChanges();
        }
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Category" : "";
            ViewBag.SortingCD = String.IsNullOrEmpty(Sorting_Order) ? "ClosureDate" : "ClosureDate_Desc";
            ViewBag.SortingFD = String.IsNullOrEmpty(Sorting_Order) ? "FinalDate" : "FinalDate_Desc";
            ViewBag.SortingStatus = String.IsNullOrEmpty(Sorting_Order) ? "Status_Category" : "Status_Desc";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;

            var category = from cate in db.Categories select cate;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                category = category.Where(cate => cate.categoryName.ToUpper().Contains(Search_Data.ToUpper()));
            }
            switch (Sorting_Order)
            {
                case "Name_Category":
                    category = category.OrderByDescending(cate => cate.categoryName);
                    break;
                case "ClosureDate":
                    category = category.OrderBy(cate => cate.closureDate);
                    break;
                case "ClosureDate_Desc":
                    category = category.OrderByDescending(cate => cate.closureDate);
                    break;
                case "FinalDate":
                    category = category.OrderBy(cate => cate.finalDate);
                    break;
                case "FinalDate_Desc":
                    category = category.OrderByDescending(cate => cate.finalDate);
                    break;
                case "Status_Category":
                    category = category.OrderBy(cate => cate.Cstatus);
                    break;
                case "Status_Desc":
                    category = category.OrderByDescending(cate => cate.Cstatus);
                    break;
                default:
                    category = category.OrderBy(cate => cate.categoryName);
                    break;
            }
            int Size_Of_Page = 5;
            int No_Of_Page = (Page_No ?? 1);

            return View(category.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoryID,categoryName,closureDate,finalDate,Cstatus")] Categories categories)
        {
            bool check = true;
            if (categories.finalDate < categories.closureDate)
            {
                check = false;
            }
            if (ModelState.IsValid)
            {
                if (check)
                {
                    db.Categories.Add(categories);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.message = "False";
                }
            }

            return View(categories);
        }
        public JsonResult CheckCate(string categoryName)
        {
            Categories categories = new Categories();

            using (var context = new ASMWebTest1Entities())
            {
                categories = context.Categories.Where(a => a.categoryName.ToLower() == categoryName.ToLower()).FirstOrDefault();
            }


            bool status;
            if (categories != null)
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

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoryID,categoryName,closureDate,finalDate,Cstatus")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categories categories = db.Categories.Find(id);
            db.Categories.Remove(categories);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult ExportCSV()
        {
            List<object> ideas = (from a in db.Ideas.ToList()
                                  join b in db.Categories.ToList() on a.categoryID equals b.categoryID
                                  where b.finalDate < DateTime.Now
                                  select new[] { a.ideaID.ToString(),
                                                     a.ideaTitle.ToString(),
                                                     a.creatAt.ToString(),
                                                     a.urlFile.ToString(),
                                                     a.description.ToString(),
                                                     a.categoryID.ToString(),
                                                     a.likes.ToString(),
                                                     a.dislikes.ToString(),
                                                     a.Iname.ToString(),
                                                     a.CountViews.ToString()
                                }).ToList<object>();
            ideas.Insert(0, new string[10] { "Ideas ID", "Idea Title", "Create At", "Url File", "Description", "Category Id", "Likes", "Dislikes", "User Name", "Views", });
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ideas.Count; i++)
            {
                string[] idea = (string[])ideas[i];
                for (int j = 0; j < idea.Length; j++)
                {
                    //Append data with separator.
                    sb.Append(idea[j] + ',');
                }

                //Append new line character.
                sb.Append("\r\n");

            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Ideas.csv");
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
