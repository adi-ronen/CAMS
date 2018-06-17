using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CAMS.Models;

namespace CAMS.Controllers
{


    public class NotificationsController : BaseController
    {

        private CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        // GET: Notifications
        public ActionResult Index()
        {
            try
            {
                int userId = (int)Session["UserId"];
                User user = db.Users.Find(userId);
                if (user != null)
                {
                    return View(new NotificationViewModel(user, this));
                }
                return RedirectToAction("Login", "Account");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

        }

        // GET: Notifications/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       

        // GET: Notifications/Edit/5
        public ActionResult Edit(int id)
        {
            return View(db.Users.Find(id));
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                User user = db.Users.Find(id);
                user.DisconnectedPeriod = Convert.ToInt32(Request["DisconnectedPeriod"].ToString());
                user.NotActivePeriod = Convert.ToInt32(Request["NotActivePeriod"].ToString());
                user.NotificationFrequency = (NotificationFrequency)Convert.ToByte(Request["NotificationFrequency"].ToString());
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(db.Users.Find(id));
            }
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Notifications/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Chart()
        { //TBD - CHNGE CHART
            var key = new Chart(width: 300, height: 300)
                .AddTitle("Employee Chart")
                .AddSeries(
                chartType: "Bubble",
                name: "Employee",
                xValue: new[] { "Peter", "Andrew", "Julie", "Dave" },
                yValues: new[] { "2", "7", "5", "3" });

            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
    }
}
