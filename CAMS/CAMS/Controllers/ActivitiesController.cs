using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAMS.Models;
using static CAMS.Constant;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CAMS.Controllers
{
    public class ActivitiesController : BaseController
    {
        private readonly object syncLock = new object();

        // GET: Activities
        public ActionResult Index()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                var activities = db.Activities.Include(a => a.Computer);
                return View(activities.ToList());
            }
        }

        // GET: Activities/Details/5
        public ActionResult Details(DateTime id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Activity activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }
                return View(activity);
            }
        }

        // GET: Activities/Create
        public ActionResult Create()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                //TBD- get all the labs that the user can create a report with
                ViewBag.ComputerId = new SelectList(db.Computers, "ComputerId", "MAC");
                return View();
            }
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Login,UserName,Logout,Mode,ComputerId")] Activity activity)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                //TBD- bind with the report details... 
                if (ModelState.IsValid)
                {
                    db.Activities.Add(activity);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                ViewBag.ComputerId = new SelectList(db.Computers, "ComputerId", "MAC", activity.ComputerId);
                return View(activity); //TBD- view report? HOW?!
            }
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(DateTime id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Activity activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ComputerId = new SelectList(db.Computers, "ComputerId", "MAC", activity.ComputerId);
                return View(activity);
            }
        }

        internal List<int> GetLabIds()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.Select(e => e.LabId).ToList();
            }
        }

        internal List<Computer> GetLabComputers(int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);
                return lab.Computers.ToList();
            }
        }
        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Login,UserName,Logout,Mode,ComputerId")] Activity activity)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ComputerId = new SelectList(db.Computers, "ComputerId", "MAC", activity.ComputerId);
                return View(activity);
            }
        }

        internal string GetCompDomain(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(computerId);
                return comp.Lab.Department.Domain;
            }
        }
        

        // GET: Activities/Delete/5
        public ActionResult Delete(DateTime id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Activity activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }
                return View(activity);
            }
        }

        internal string GetCurrentComputerUser(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Activity> act = db.Activities.Where(e => e.ComputerId.Equals(computerId) && !e.Logout.HasValue).ToList();
                if (act.Count > 0)
                    return act.First().UserName;
                else
                    return "";

            }
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Activity activity = db.Activities.Find(id);
                db.Activities.Remove(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        internal void UpdateLabSchedule(int labId, string classes)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);
                lab.TodaysClasses = classes;
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        

        public void CreateNewActivity(int compId, ActivityType mode, string userName)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                // Activity act = new Activity();
                Computer comp = db.Computers.Find(compId);
                string uName = null;
                if (userName != null)
                    uName = userName;
                Activity act = new Activity
                {
                    UserName = uName,
                    Mode = mode,
                    Login = DateTime.Now,
                    Weekend = IsWeekend(DateTime.Now.DayOfWeek),
                    ComputerId = comp.ComputerId
                };
                db.Activities.Add(act);
                comp.Activities.Add(act);
                db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
                db.SaveChanges();
            }
        }

        public void CreateNewClassActivity(int labId, DateTime start, DateTime end)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                // Activity act = new Activity();
                Lab lab = db.Labs.Find(labId);
                
                foreach (var item in lab.Computers)
                {
                    Activity act = new Activity
                    {
                        Mode = ActivityType.Class,
                        Login = start,
                        Logout = end,
                        Weekend = false,
                        ComputerId = item.ComputerId
                    };
                    db.Activities.Add(act);
                 //   item.Activities.Add(act);
                 //   db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
                    db.SaveChanges();
                }
                
            }
        }

        public int FindLabID(string building,string room)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                var labs=db.Labs.Where(e => e.Building.Contains(building) && e.RoomNumber.Remove('-').Contains(room)).ToList();
                return labs.First().LabId;
            }
        }


        private bool IsWeekend(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek.Equals(DayOfWeek.Friday) || dayOfWeek.Equals(DayOfWeek.Saturday));

        }

       
        public void CloseActivity(int compId)
        {
            
            ExecudeCommand("UPDATE Activities SET Logout = '" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "' WHERE ComputerId = '" + compId + "' AND Logout is null; ");

        }

        public void SplitActivity(int compId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Activity> acts = db.Activities.Where(e => e.ComputerId.Equals(compId) && !e.Logout.HasValue).ToList();
                foreach (var act in acts)
                {
                    if(!act.Login.Date.Equals(DateTime.Now.Date))
                    {
                        act.Logout = DateTime.Now.Date.AddTicks(-1);
                        Activity newAct = new Activity
                        {
                            Login = DateTime.Now.Date
                        };
                        newAct.Weekend = IsWeekend(newAct.Login.DayOfWeek);
                        newAct.ComputerId = act.ComputerId;
                        newAct.Mode = act.Mode;
                        if (act.UserName != null)
                            act.UserName = act.UserName;
                        db.Activities.Add(newAct);
                        db.SaveChanges();
                    }
                }
               
            }
        }

        internal List<Lab> GetAllLabs()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.ToList();
            }
        }

        internal void ClearLabsSchedule()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                foreach (var labId in db.Labs.Select(e=>e.LabId).ToList())
                {
                    UpdateLabSchedule(labId, "");
                }
                
            }
        }
    }
}
