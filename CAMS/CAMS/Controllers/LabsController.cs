using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CAMS.Models;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace CAMS.Controllers
{
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
            using (var db = new CAMS_DatabaseEntities())
            {
                var Labs = db.Labs.Include(e => e.Department).Include(e=>e.Computers);
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
        }




        // GET: Labs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(id);
                db.Entry(lab).Collection(e=>e.Computers).Load();
                if (lab == null)
                {
                    return HttpNotFound();
                }
                return View(new LabDetailsViewModel(lab, this));
            }
        }

        // GET: Labs/Create
        public ActionResult Create()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                //Tuple<List<Department>, List<string>> DepartmentsAndBuildings;
                List<Department> departments = db.Departments.ToList();
                List<string> buildings = db.Labs.Select(lab => lab.Building).Distinct().ToList();
                return View(new object[] { departments, buildings });
            }
        }

        // POST: Labs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Building,RoomNumber,DepartmentId")] Lab lab)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                lab.LabId = db.Labs.Max(e => e.LabId) + 1;
                if (ModelState.IsValid)
                {
                    db.Labs.Add(lab);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
                return View(lab);
            }
        }

        // GET: Labs/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(id);
                if (lab == null)
                {
                    return HttpNotFound();
                }
                db.Entry(lab).Collection(e => e.Computers).Load();
                db.Entry(lab).Reference(e => e.Department).Load();

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
                return View(new LabDetailsViewModel(lab, this));
            }
        }

        // POST: Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabId,TodaysClasses,Building,RoomNumber,DepartmentId,Computers")] Lab lab)
        {
            using (var db = new CAMS_DatabaseEntities())
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
        }

        // GET: Labs/Delete/5
        public ActionResult Delete(int? id)
        {
            using (var db = new CAMS_DatabaseEntities())
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
        }

        // POST: Labs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                DeleteLab(id);
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


        
        public void AddComputerToLab(int compId, int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(compId);
                //already in lab- nothing to update
                if (comp.CurrentLab == labId)
                    return;
                ComputerLab cL = new ComputerLab
                {
                    ComputerId = compId,
                    LabId = labId,
                    Entrance = DateTime.Now,
                    Exit = null
                };
                comp.CurrentLab = labId;
                db.ComputerLabs.Add(cL);

                db.SaveChanges();
            }

        }

        public bool SaveLabEdit(List<Computer> comps, int labId, string roomNumber, string building, int ComputerSize)
        {

            try
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    Lab lab = db.Labs.Find(labId);

                    lab.RoomNumber = roomNumber;
                    lab.Building = building;
                    lab.ComputerSize = ComputerSize;
                    var privLabComputers = lab.Computers.ToList();
                    //computers in the lab that was removed (not in the current computer list)
                    foreach (var item in privLabComputers.Except(comps))
                    {
                        RemoveComputerFromLab(item.ComputerId, lab.LabId);
                    }
                    //computers added to the lab (not in the lab computer list)
                    foreach (var item in comps.Except(privLabComputers))
                    {
                        AddComputerToLab(item.ComputerId, lab.LabId);
                    }

                    //computers to update
                    foreach (var item in comps.Intersect(privLabComputers))
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.Entry(lab).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex) { }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        //public bool SaveLabEdit(List<Computer> comps, int labId,string roomNumber,string building)
        //{
        //    using (var tr = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            Lab lab = db.Labs.Find(labId);

        //            lab.RoomNumber = roomNumber;
        //            lab.Building = building;
        //            var privLabComputers = lab.Computers.ToList();
        //            //computers in the lab that was removed (not in the current computer list)
        //            foreach (var item in privLabComputers.Except(comps))
        //            {
        //                RemoveComputerFromLab(item.ComputerId, lab.LabId);
        //            }
        //            //computers added to the lab (not in the lab computer list)
        //            foreach (var item in comps.Except(privLabComputers))
        //            {
        //                AddComputerToLab(item.ComputerId, lab.LabId);
        //            }

        //            //computers to update
        //            foreach (var item in comps.Intersect(privLabComputers))
        //            {
        //                db.Entry(item).State = EntityState.Modified;
        //            }
        //            db.Entry(lab).State = EntityState.Modified;
        //            try
        //            {
        //                db.SaveChanges();
        //            }
        //            catch (DbUpdateConcurrencyException ex) { }
        //            tr.Commit();
        //            return true;
        //        }
        //         catch (Exception)
        //        {
        //            tr.Rollback();
        //            return false;
        //        }
        //    }
        //}

        // POST: Labs/Update
        [HttpPost]
        public ActionResult Update(Dictionary<string, string> computers, string LabId, string RoomNumber, string Building, string ComputerSize)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(Convert.ToInt32(LabId));
                List<Computer> comps = new List<Computer>();

                foreach (var item in computers)
                {
                    if (item.Key.Split(',').Length != 2)
                        break;
                    Computer computer;
                    //if computer have an id
                    if (int.TryParse(item.Key.Split(',')[1], out int n))
                    {
                        computer = db.Computers.Find(n);
                    }
                    else
                    {
                        computer = CreateComputer(item.Key.Split(',')[0], lab.Department.Domain);
                    }
                    //save computer new location
                    computer.LocationInLab = item.Value;
                    db.Entry(computer).State = EntityState.Modified;
                    db.SaveChanges();
                    comps.Add(computer);
                }

                if (!SaveLabEdit(comps, lab.LabId, RoomNumber.Trim(), Building, Convert.ToInt32(ComputerSize)))
                {
                    //retry
                    SaveLabEdit(comps, lab.LabId, RoomNumber.Trim(), Building, Convert.ToInt32(ComputerSize));
                }
                return View(new LabDetailsViewModel(lab, this));
            }

        }


    }
}
