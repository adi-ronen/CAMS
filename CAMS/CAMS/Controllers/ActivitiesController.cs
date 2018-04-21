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

namespace CAMS.Controllers
{
    public class ActivitiesController : BaseController
    {
        // GET: Activities
        public ActionResult Index()
        {
            var activities = db.Activities.Include(a => a.Computer);
            return View(activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(DateTime id)
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

        // GET: Activities/Create
        public ActionResult Create()
        {
            //TBD- get all the labs that the user can create a report with
            ViewBag.ComputerId = new SelectList(db.Computers, "ComputerId", "MAC");
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Login,UserName,Logout,Mode,ComputerId")] Activity activity)
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

        // GET: Activities/Edit/5
        public ActionResult Edit(DateTime id)
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

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Login,UserName,Logout,Mode,ComputerId")] Activity activity)
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

        // GET: Activities/Delete/5
        public ActionResult Delete(DateTime id)
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

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        internal void UpdateLabSchedule(Lab lab, string classes)
        {
            lab.TodaysClasses = classes;
            db.Entry(lab).State = EntityState.Modified;
            db.SaveChanges();
        }
        

        public void CreateNewActivity(Computer comp, ActivityMode mode, string userName)
        {
            Activity act = new Activity();
            if (userName != null)
                act.UserName = userName;
            act.Mode = (byte)mode;
            act.Login = DateTime.Now;
            act.Weekend = IsWeekend(act.Login.DayOfWeek);
            act.ComputerId = comp.ComputerId;
            act.Computer = comp;
            db.Activities.Add(act);
            comp.Activities.Add(act);
            db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
            db.SaveChanges();
        }

        private bool IsWeekend(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek.Equals(DayOfWeek.Friday) || dayOfWeek.Equals(DayOfWeek.Saturday));

        }

       
        public void CloseActivity(Activity act)
        {
            act.Logout = DateTime.Now;
            db.Entry(act).State = EntityState.Modified; //same as above
            db.SaveChanges();

        }

        public Activity SplitActivity(Activity act)
        {
            act.Logout = DateTime.Now.Date.AddTicks(-1);
            Activity newAct = new Activity();
            newAct.Login = DateTime.Now.Date;
            newAct.Weekend = IsWeekend(newAct.Login.DayOfWeek);
            newAct.ComputerId = act.ComputerId;
            newAct.Mode = act.Mode;
            if (act.UserName != null)
                act.UserName = act.UserName;
            db.Activities.Add(newAct);
            db.SaveChanges();
            return newAct;
        }

        internal List<Lab> GetAllLabs()
        {
            return db.Labs.Select(e => e).ToList<Lab>();
        }
    }
}
