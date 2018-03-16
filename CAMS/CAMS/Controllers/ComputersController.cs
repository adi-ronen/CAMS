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
    public class ComputersController : Controller
    {
        private CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        // GET: Computers
        public ActionResult Index()
        {
            var computers = db.Computers.Include(c => c.Lab);
            return View(computers.ToList());
        }

        // GET: Computers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }

            return View(computer);
        }
        public Activity LastActivityDetails(int id)
        {
            Computer computer = db.Computers.Find(id);
            return LastActivityDetails(computer);
        }
        public Activity LastActivityDetails(Computer comp)
        {
            if (comp == null)
            {
                return null;
            }
            try
            {
                // return computer.Activities.Select(e => e).Where(e => e.Mode != ActivityMode.Class.ToString() && e.Logout.Equals(null)).Last();
                return comp.Activities.Select(e => e).Where(e => e.Mode != ActivityMode.Class.ToString() && e.Logout.Equals(null)).Last();

            }
            catch (InvalidOperationException)
            {
                //if no element was found that means the cumputer is currently in On state with no user loged in

                return null;
            }
        }



        // GET: Computers/Create
        public ActionResult Create()
        {
            ViewBag.CurrentLab = new SelectList(db.Labs, "LabId");
            return View();
        }

        // POST: Computers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComputerId,MAC,ComputerName,CurrentLab,LocationInLab")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                db.Computers.Add(computer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CurrentLab = new SelectList(db.Labs, "LabId", "LabName", computer.CurrentLab);
            return View(computer);
        }

        public void CreateNewActivity(Computer comp, ActivityMode mode, string userName)
        {
            Activity act = new Activity();
            if (userName != null)
                act.UserName = userName;
            act.Mode = mode.ToString();
            act.Login = DateTime.Now;
            act.ComputerId = comp.ComputerId;
            act.Computer = comp;
            db.Activities.Add(act);
            comp.Activities.Add(act);
            db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
            db.SaveChanges();
        }

        public void CloseActivity(Activity act)
        {
            act.Logout= DateTime.Now;
            db.Entry(act).State = EntityState.Modified; //same as above
            db.SaveChanges();

        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrentLab = new SelectList(db.Labs, "LabId", "LabName", computer.CurrentLab);
            return View(computer);
        }

        // POST: Computers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComputerId,MAC,ComputerName,CurrentLab,LocationInLab")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(computer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CurrentLab = new SelectList(db.Labs, "LabId", "LabName", computer.CurrentLab);
            return View(computer);
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            return View(computer);
        }

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Computer computer = db.Computers.Find(id);
            db.Computers.Remove(computer);
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
    }
}
