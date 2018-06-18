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
            int userId = GetConnectedUser();
            if (userId != -1)
            {
                User user = db.Users.Find(userId);
                if (user != null)
                {
                    return View(new NotificationViewModel(user, this));
                }
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        // GET: Notifications/Edit
        public ActionResult Edit()
        {
            int userId = GetConnectedUser();
            if (userId != -1)
            {
                return View(db.Users.Find(userId));
            }
            return RedirectToAction("Index");

        }

        // POST: Notifications/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                int userId = GetConnectedUser();
                if (userId != -1)
                {
                    User user = db.Users.Find(userId);
                    user.DisconnectedPeriod = Convert.ToInt32(Request["DisconnectedPeriod"].ToString());
                    user.NotActivePeriod = Convert.ToInt32(Request["NotActivePeriod"].ToString());
                    user.NotificationFrequency = (NotificationFrequency)Convert.ToByte(Request["NotificationFrequency"].ToString());
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
