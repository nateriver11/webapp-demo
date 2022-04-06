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
    public class DashboardController : Controller
    {
        private ASMWebTest1Entities db = new ASMWebTest1Entities();

        public ActionResult Index()
        {
            var totalIdeas = db.GetTotalIdeas(2022).ToList();
            List<string> departmentName1 = new List<string>();
            List<int?> nIdeas = new List<int?>();
            foreach (var ideas in totalIdeas)
            {
                departmentName1.Add(ideas.departmenName);
                nIdeas.Add(ideas.TotalIdeas);
            };
            ViewBag.DepartmentName1 = departmentName1;
            ViewBag.Ideas = nIdeas;

            var totalContributors = db.GetTotalContributors();
            List<string> departmentName2 = new List<string>();
            List<int?> nContributors = new List<int?>();
            foreach (var contributors in totalContributors)
            {
                departmentName2.Add(contributors.Department);
                nContributors.Add(contributors.TotalContributors);
            };
            ViewBag.DepartmentName2 = departmentName1;
            ViewBag.Contributors = nContributors;
            var mostPopular = from a in db.view_d select a;
            //GET MOST VIEW IDEA
            var mostViewed = from b in db.view_e select b;
            ViewBag.MostPopular = mostPopular;
            ViewBag.MostViewed = mostViewed;
            int countIdeas = (from c in db.Ideas select c).Count();

            ViewBag.countIdea = countIdeas; 
            return View();
        }

    }

}