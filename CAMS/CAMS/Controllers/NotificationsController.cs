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

        // GET: Notifications
        public ActionResult Index()
        {
            try
            {
                using (var db = new CAMS_DatabaseEntities())
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
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        internal List<Computer> GetLabComputers(int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Computer> comps = db.Labs.Find(labId).Computers.ToList();
                foreach (var item in comps)
                {
                    db.Entry(item).Collection(e => e.ComputerLabs).Load();

                }
                return comps;

            }
        }

        internal List<Department> GetUserViewDepartments(int userId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (IsSuperUser())
                    return GetAllDepartments();
                else
                {
                    List<Department> dep=db.UserDepartments.Where(e => e.UserId.Equals(userId)).Select(e => e.Department).ToList();
                    foreach (var item in dep)
                    {
                        db.Entry(item).Collection(e => e.Labs).Load();
                    }
                    return dep;
                }
            }
        }

        internal List<Activity> GetCurrentDisconnectedActivity(int compId,DateTime disconectedFrom)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(compId);
                return comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == ActivityType.Off && e.Login <= disconectedFrom).ToList();
            }
        }

        // GET: Notifications/Edit
        public ActionResult Edit()
        {
            try
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    int userId = GetConnectedUser();
                    if (userId != -1)
                    {
                        return View(db.Users.Find(userId));
                    }
                    return RedirectAcordingToLogin();
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        internal Activity GetUserActivity(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
               return db.Computers.Find(computerId).Activities.Where(e => !e.Logout.HasValue && e.Mode == ActivityType.User).ToList().FirstOrDefault();
            }

        }

        // POST: Notifications/Edit
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                using (var db = new CAMS_DatabaseEntities())
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
                        return RedirectToAction("Index");

                    }
                    return RedirectAcordingToLogin();
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        internal DateTime? GetLastLogOutTime(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Computers.Find(computerId).Activities.Where(e => e.Mode == ActivityType.User).Max(e => e.Logout);
            }

        }
    }
}
