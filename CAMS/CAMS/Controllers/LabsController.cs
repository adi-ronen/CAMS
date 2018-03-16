using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAMS.Models;

namespace CAMS.Controllers
{
    public class LabsController : Controller
    {
        private CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        // GET: Labs
        public ActionResult Index()
        {
            var labs = db.Labs.Include(l => l.Department);
            return View(labs.ToList());
        }

        // GET: Labs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            return View(lab);
        }

        // GET: Labs/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Labs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LabId,LabName,Building,RoomNumber,DepartmentId")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                db.Labs.Add(lab);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
            return View(lab);
        }

        // GET: Labs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
            return View(lab);
        }

        // POST: Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabId,LabName,Building,RoomNumber,DepartmentId")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
            return View(lab);
        }

        // GET: Labs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            return View(lab);
        }

        // POST: Labs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lab lab = db.Labs.Find(id);
            db.Labs.Remove(lab);
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
