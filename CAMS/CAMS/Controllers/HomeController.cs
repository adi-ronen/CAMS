using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (CAMS_DatabaseEntities db = new CAMS_DatabaseEntities())
            {
                var result = from a in db.Labs
                             select new
                             {
                                 a.LabName,
                                 a.RoomNumber,
                                 a.Building,
                                 a.Computers.Count,
                                 a.Department
                             };
                return View(result.ToList());
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}