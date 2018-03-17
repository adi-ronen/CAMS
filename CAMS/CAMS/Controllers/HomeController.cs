using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAMS.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";
            List<Models.Computer> list = new List<Computer>();
            for (int i = 0; i <= 30; i++)
            {
                Computer c = new Computer();
                c.ComputerName = "lb-107-" + i;
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