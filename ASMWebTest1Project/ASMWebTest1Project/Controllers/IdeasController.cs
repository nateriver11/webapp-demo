using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ASMWebTest1Project.Models;
using Ionic.Zip;
using PagedList;

namespace ASMWebTest1Project.Controllers
{
    [Authorize]
    public class IdeasController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        // GET: Ideas
        public ActionResult IndexGuest(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingTitle= String.IsNullOrEmpty(Sorting_Order) ? "Title_Idea" : "";
            ViewBag.SortingCreateOn = String.IsNullOrEmpty(Sorting_Order) ? "CreateOn" : "CreateOn_Desc";
            ViewBag.SortingCategory = String.IsNullOrEmpty(Sorting_Order) ? "Category_Name" : "Category_Name_Desc";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;

            var idea = from ide in db.Ideas select ide;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                idea = idea.Where(ide => ide.ideaTitle.ToUpper().Contains(Search_Data.ToUpper()));
            }
            switch (Sorting_Order)
            {
                case "Title_Idea":
                    idea = idea.OrderByDescending(ide => ide.ideaTitle);
                    break;
                case "CreateOn":
                    idea = idea.OrderBy(ide => ide.creatAt);
                    break;
                case "CreateOn_Desc":
                    idea = idea.OrderByDescending(ide => ide.creatAt);
                    break;
                case "Category_Name":
                    idea = idea.OrderBy(ide => ide.Categories.categoryName);
                    break;
                case "Category_Name_Desc":
                    idea = idea.OrderByDescending(ide => ide.Iname);
                    break;
                default:
                    idea = idea.OrderBy(ide => ide.Categories.categoryName);
                    break;
            }
            int Size_Of_Page = 5;
            int No_Of_Page = (Page_No ?? 1);
            return View(idea.Where(i => i.Iname == User.Identity.Name).ToPagedList(No_Of_Page, Size_Of_Page));

        }
        /*public ActionResult IndexGuest()
        {
            var ideas = db.Ideas.Include(i => i.Categories);
            return View(ideas.ToList());
        }*/
        public ActionResult TestIndex(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingTitle = String.IsNullOrEmpty(Sorting_Order) ? "Title_Idea" : "";
            ViewBag.SortingCreateOn = String.IsNullOrEmpty(Sorting_Order) ? "CreateOn" : "CreateOn_Desc";
            ViewBag.SortingCategory = String.IsNullOrEmpty(Sorting_Order) ? "Category_Name" : "Category_Name_Desc";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;

            var idea = from ide in db.Ideas select ide;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                idea = idea.Where(ide => ide.ideaTitle.ToUpper().Contains(Search_Data.ToUpper()));
            }
            switch (Sorting_Order)
            {
                case "Title_Idea":
                    idea = idea.OrderByDescending(ide => ide.ideaTitle);
                    break;
                case "CreateOn":
                    idea = idea.OrderBy(ide => ide.creatAt);
                    break;
                case "CreateOn_Desc":
                    idea = idea.OrderByDescending(ide => ide.creatAt);
                    break;
                case "Category_Name":
                    idea = idea.OrderBy(ide => ide.Categories.categoryName);
                    break;
                case "Category_Name_Desc":
                    idea = idea.OrderByDescending(ide => ide.Iname);
                    break;
                default:
                    idea = idea.OrderBy(ide => ide.Categories.categoryName);
                    break;
            }
            int Size_Of_Page = 5;
            int No_Of_Page = (Page_No ?? 1);
            return View(idea.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        // GET: Ideas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ideas ideas = db.Ideas.Find(id);
            if (DateTime.Now > db.Categories.Find(ideas.categoryID).finalDate)
            {

                return View("~/Views/Shared/_MainMenu.cshtml");

            }
            if (ideas == null)
            {
                return HttpNotFound();
            }
            Ideas update = db.Ideas.ToList().Find(u => u.ideaID == id);
            update.CountViews += 1;
            db.SaveChanges();
            return View(ideas);
        }
        public ActionResult Views(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ideas ideas = db.Ideas.Find(id);
            if (ideas == null)
            {
                return HttpNotFound();
            }
            Ideas update = db.Ideas.ToList().Find(u => u.ideaID == id);
            update.CountViews += 1;
            db.SaveChanges();
            return View(ideas);
        }

        // GET: Ideas/Create
        public ActionResult Create()
        {
            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "categoryName");
            
            return View();
        }
        
        // POST: Ideas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ideaID,ideaTitle,creatAt,fileI,urlFile,description,likes,dislikes,categoryID,Iname, incognito,CountViews")] Ideas ideas, HttpPostedFileBase file)
        {
            
            bool notifi = true;
          
            if (file != null && file.ContentLength > 0)
            {
                ideas.fileI = new byte[file.ContentLength]; // image stored in binary formate
                file.InputStream.Read(ideas.fileI, 0, file.ContentLength);
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string UrlFile = Server.MapPath("~/FileI/" + fileName);
                file.SaveAs(UrlFile);

                ideas.urlFile = "FileI/" + fileName;
            }
            if(ideas.creatAt > db.Categories.Find(ideas.categoryID).closureDate)
            {
                notifi = false;

            }
            if (ModelState.IsValid)
            {
                if (notifi)
                {
                    var user = db.Information.FirstOrDefault(x => x.Irole == "Quality Assurance Coordinator").email;
                    string subject = "Notifaction";
                    int ids = db.Ideas.Max(item => item.ideaID + 1);
                    if(ids == 0)
                    {
                        ids = 1;
                    }
                    string body = ideas.Iname + " just sent the idea. Link:" + "https://localhost:44345/Ideas/Details/" + ids;
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
                    db.Ideas.Add(ideas);
                    db.SaveChanges();
                    return RedirectToAction("IndexGuest");
                }
                else
                {
                    ViewBag.message = "Create is fail, category is close!";
                }
            }

            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "categoryName",ideas.categoryID);


            return View(ideas);
        }

        public ActionResult Download(string fileName)
        {
                string fullPath = Path.Combine(Server.MapPath("~/"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult DownloadAll()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/FileI/"));
            List<FileModel> files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel()
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                });
            }

            return View(files);
        }

        [HttpPost]
        public ActionResult DownloadAll (List<FileModel> files)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("Filel");
                foreach (FileModel file in files)
                {
                    if (file.IsSelected)
                    {
                        zip.AddFile(file.FilePath, "Filel");
                    }
                }
                string zipName = String.Format("FilesZip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", zipName);
                }
            }


        }

        // GET: Ideas/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ideas ideas = db.Ideas.Find(id);
            if (ideas == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "categoryName", ideas.categoryID);
            
            return View(ideas);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ideaID,ideaTitle,creatAt,fileI,urlFile,likes,dislikes,description,categoryID,Iname")] Ideas ideas, HttpPostedFileBase editFile)
        {
            bool notifi = true;
            if (ideas.creatAt > db.Categories.Find(ideas.categoryID).closureDate)
            {
                notifi = false;

            }
            if (ModelState.IsValid)
            {
                if (notifi)
                {
                    Ideas modifyProduct = db.Ideas.Find(ideas.ideaID);
                    if (modifyProduct != null)
                    {
                        if (editFile != null && editFile.ContentLength > 0)
                        {
                            modifyProduct.fileI = new byte[editFile.ContentLength];
                            editFile.InputStream.Read(modifyProduct.fileI, 0, editFile.ContentLength);
                            string fileName = System.IO.Path.GetFileName(editFile.FileName);
                            string UrlFile = Server.MapPath("~/FileI/" + fileName);
                            editFile.SaveAs(UrlFile);

                            modifyProduct.urlFile = "FileI/" + fileName;
                        }
                    }
                    Ideas update = db.Ideas.ToList().Find(u => u.ideaID == ideas.ideaID);
                    update.creatAt = DateTime.Now;
                    db.Entry(modifyProduct).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("IndexGuest");
                }
                else
                {
                    ViewBag.message = "False";
                }
            }
            return View(ideas);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ideas ideas = db.Ideas.Find(id);
            if (ideas == null)
            {
                return HttpNotFound();
            }
            return View(ideas);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ideas ideas = db.Ideas.Find(id);
            db.Ideas.Remove(ideas);
            db.SaveChanges();
            return RedirectToAction("IndexGuest");
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
