using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CAMS.Models;
using PagedList;
using System.Collections.Generic;


namespace CAMS.Controllers
{
    [RequireHttps]
    public class LabsController : BaseController

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
            //return View(Labs.ToPagedList(pageNumber, pageSize));
            return View(new LabsViewModel(Labs.ToPagedList(pageNumber, pageSize), this));
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
            return View(new LabDetailsViewModel(lab, this));
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
            return View(new LabDetailsViewModel(lab, this));
        }

        // POST: Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabId,TodaysClasses,Building,RoomNumber,DepartmentId,Computers")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                lab.RoomNumber = lab.RoomNumber.Trim();
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
            while (lab.Computers.Count>0)
            {
                RemoveComputerFromLab(lab.Computers.First().ComputerId, lab.LabId);
            }

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

       

        public void RemoveComputerFromLab(int compId, int labId)
        {
            Computer comp = db.Computers.Find(compId);
            //computer not in lab- nothing to update
            if (comp.CurrentLab != labId)
                return;
            ComputerLab cL = comp.ComputerLabs.Select(e => e).Where(e => e.Exit.Equals(null)).Where(e=>e.LabId.Equals(labId)).Last();
            if (cL == null)
                throw new Exception("no record of computer in lab");
            cL.Exit= DateTime.Now;
            //lab.RoomNumber = lab.RoomNumber.Trim();
            comp.CurrentLab = null;
            //Lab lab = comp.Lab;
            //lab.Computers.Remove(comp);
            //comp.Lab = null;

            //lab.Computers.Remove(comp);
            //comp.CurrentLab = null;
            //db.Entry(cL).State = EntityState.Modified;
            //db.Entry(comp).State = EntityState.Modified;
            //db.Entry(lab).State = EntityState.Modified;
            db.Computers.Attach(comp);
            db.ComputerLabs.Attach(cL);
            db.SaveChanges();


        }
        public void AddComputerToLab(int compId, int labId)
        {
            Computer comp = db.Computers.Find(compId);
            //already in lab- nothing to update
            if (comp.CurrentLab == labId)
                return;
            ComputerLab cL = new ComputerLab();
            cL.ComputerId = compId;
            cL.LabId = labId;
            cL.Entrance = DateTime.Now;
            cL.Exit = null;
            comp.CurrentLab = labId;
            db.ComputerLabs.Add(cL);

            db.SaveChanges();

        }


        

        public void SaveLabEdit(List<Computer> comps, Lab lab)
        {
            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    //computers in the lab that was removed (not in the current computer list)
                    foreach (var item in lab.Computers.Except(comps))
                    {
                        RemoveComputerFromLab(item.ComputerId, lab.LabId);
                    }
                    //computers added to the lab (not in the lab computer list)
                    foreach (var item in comps.Except(lab.Computers))
                    {
                        AddComputerToLab(item.ComputerId, lab.LabId);
                    }

                    //computers to update
                    foreach (var item in comps.Intersect(lab.Computers))
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                }
            }
        }

        // POST: Labs/Update
        [HttpPost]
        public ActionResult Update(Dictionary<string, string> computers, string LabId, string RoomNumber, string Building)
        {
            return View();
        }


    }
}
