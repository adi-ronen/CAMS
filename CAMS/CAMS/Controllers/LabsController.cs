using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CAMS.Models;
using PagedList;

namespace CAMS.Controllers
{
    [RequireHttps]
    public class LabsController : Controller

    {

        // GET: Labs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DepSortParm = String.IsNullOrEmpty(sortOrder) ? "dep_desc" : "";
            ViewBag.BuildingSortParm = sortOrder == "Building" ? "building_desc" : "Building";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var Labs = from l in db.Labs
                           select l;
            if (!String.IsNullOrEmpty(searchString))
            {
                Labs = Labs.Where(l => l.Department.DepartmentName.Contains(searchString)
                                       || l.Building.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "dep_desc":
                    Labs = Labs.OrderByDescending(l => l.Department.DepartmentName);
                    break;
                case "Building":
                    Labs = Labs.OrderBy(l => l.Building);
                    break;
                case "building_desc":
                    Labs = Labs.OrderByDescending(l => l.Building);
                    break;
                default:
                    Labs = Labs.OrderBy(l => l.Department.DepartmentName);
                    break;
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(Labs.ToPagedList(pageNumber, pageSize));
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
            return View(new LabModel(lab,this));
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
        public ActionResult Create([Bind(Include = "LabId,TodaysClasses,Building,RoomNumber,DepartmentId")] Lab lab)
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
        public ActionResult Edit([Bind(Include = "LabId,TodaysClasses,Building,RoomNumber,DepartmentId")] Lab lab)
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
