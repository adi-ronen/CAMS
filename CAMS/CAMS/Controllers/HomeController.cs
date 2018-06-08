using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CAMS.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
 
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DepSortParm = String.IsNullOrEmpty(sortOrder) ? "dep_desc" : "";
            ViewBag.BuildingSortParm = sortOrder == "Building" ? "building_desc" : "Building";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            using (var db = new CAMS_DatabaseEntities())
            {
                var Labs = from l in db.Labs
                           select l;
                if (!String.IsNullOrEmpty(searchString))
                {
                    Labs = Labs.Where(l => l.Department.DepartmentName.Contains(searchString)
                                           || l.Building.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "dep_desc":
                        Labs = Labs.OrderByDescending(l => l.Department.DepartmentName);
                        break;
                    case "Building":
                        Labs = Labs.OrderBy(l => l.Building);
                        break;
                    case "building_desc":
                        Labs = Labs.OrderByDescending(l => l.Building);
                        break;
                    default:
                        Labs = Labs.OrderBy(l => l.Department.DepartmentName);
                        break;
                }
                int pageSize = 8;
                int pageNumber = (page ?? 1);
                return View(new LabsViewModel(Labs.ToPagedList(pageNumber, pageSize), this));
            }
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";
            List<Models.Computer> list = new List<Computer>();
            for (int i = 0; i <= 30; i++)
            {
                Computer c = new Computer
                {
                    ComputerName = "lb-107-" + i
                };
                list.Add(c);
            }
            //ViewBag.Message = ActivitiesModel.GetComputersActivity(list);
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}