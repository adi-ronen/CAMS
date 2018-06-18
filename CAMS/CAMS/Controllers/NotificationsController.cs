using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CAMS.Models;
using System.Net;

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
                    return RedirectAcordingToLogin();
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        // GET: Notifications/Edit
        public ActionResult Edit()
        {
            try
            {
                int userId = GetConnectedUser();
                if (userId != -1)
                {
                    return View(db.Users.Find(userId));
                }
                return RedirectAcordingToLogin();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: Notifications/Edit
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
                return RedirectAcordingToLogin();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}
