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
            try
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
                    var Labs = db.Labs.Include(e => e.Department).Include(e => e.Computers);
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
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }




        // GET: Labs/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                using (var db = new CAMS_DatabaseEntities())
                {
                    Lab lab = db.Labs.Find(id);
                    db.Entry(lab).Collection(e => e.Computers).Load();
                    if (lab == null)
                    {
                        return HttpNotFound();
                    }
                    return View(new LabDetailsViewModel(lab, this));
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Labs/Create
        public ActionResult Create()
        {
            try
            {
                if (IsFullAccessUser())
                {
                    using (var db = new CAMS_DatabaseEntities())
                    {
                        //Tuple<List<Department>, List<string>> DepartmentsAndBuildings;
                        List<Department> departments = db.Departments.ToList();
                        List<Department> userDepartments = new List<Department>();
                        foreach (var item in departments)
                        {
                            //user can only add labs to the departments he have FULL accsses to.
                            if (IsFullAccess(item.DepartmentId))
                                userDepartments.Add(item);
                        }

                        List<string> buildings = db.Labs.Select(lab => lab.Building).Distinct().ToList();
                        return View(new object[] { userDepartments, buildings });
                    }
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

        /// <summary>
        /// list of computers name that are allready in labs
        /// </summary>
        /// <returns></returns>
        internal List<string> ComputersInLabs()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
               return db.Computers.Where(computer => !computer.CurrentLab.Equals(null)).Select(computer => computer.ComputerName).ToList();
            }
        }

        // POST: Labs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Building,RoomNumber,DepartmentId")] Lab lab)
        {
            try
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    if (IsFullAccess(lab.DepartmentId))
                    {
                        if (ModelState.IsValid)
                        {
                            lab.ComputerSize = 50;
                            db.Labs.Add(lab);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }

                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
                        return View(lab);
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

        // GET: Labs/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
                    if (IsLimitedAccess(lab.DepartmentId))
                    {
                        db.Entry(lab).Collection(e => e.Computers).Load();
                        db.Entry(lab).Reference(e => e.Department).Load();

                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", lab.DepartmentId);
                        return View(new LabDetailsViewModel(lab, this));
                    }
                    //user have no access to edit lab
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

        // POST: Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabId,TodaysClasses,Building,RoomNumber,DepartmentId,Computers")] Lab lab)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Labs/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Lab lab = db.Labs.Find(id);
                    if (IsFullAccess(lab.DepartmentId))
                    {
                        db.Entry(lab).Reference(e => e.Department).Load();
                        if (lab == null)
                        {
                            return HttpNotFound();
                        }
                        return View(lab);
                    }
                    //user have no access to delete lab
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

        // POST: Labs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Lab lab = GetLab(id);
                if (IsFullAccess(lab.DepartmentId))
                {
                    DeleteLab(id);
                    return RedirectToAction("Index");
                }
                //user have no access to delete lab
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
       
        public bool SaveLabEdit(List<int> comps, int labId, string roomNumber, int ComputerSize)
        {

            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);

                lab.RoomNumber = roomNumber;
                lab.ComputerSize = ComputerSize;
                var privLabComputers = lab.Computers.Select(e=>e.ComputerId).ToList();
                //computers in the lab that was removed (not in the current computer list)
                foreach (var item in privLabComputers.Except(comps))
                {
                    RemoveComputerFromLab(item, lab.LabId);
                }
                //computers added to the lab (not in the lab computer list)
                foreach (var item in comps.Except(privLabComputers))
                {
                    AddComputerToLab(item, lab.LabId);
                }

                //computers to update
                //foreach (var item in comps.Intersect(privLabComputers))
                //{
                //    db.Entry(item).State = EntityState.Modified;
                //}
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
                return true;
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
        public ActionResult Update(Dictionary<string, string> computers, string LabId, string RoomNumber, string ComputerSize)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(Convert.ToInt32(LabId));
                List<int> comps = new List<int>();

                foreach (var item in computers)
                {
                    if (item.Key.Split(',').Length != 2)
                        continue;
                    Computer computer;
                    //if computer have an id
                    if (int.TryParse(item.Key.Split(',')[1], out int n))
                    {
                        computer = db.Computers.Find(n);
                    }
                    else
                    {
                        string compName = item.Key.Split(',')[0];
                        computer= GetComputer(compName);
                        if(computer==null)
                            computer = CreateComputer(compName, lab.Department.Domain);
                    }
                    //save computer new location
                    computer.LocationInLab = item.Value;
                    db.Entry(computer).State = EntityState.Modified;
                    db.SaveChanges();
                    comps.Add(computer.ComputerId);
                }

                SaveLabEdit(comps, lab.LabId, RoomNumber.Trim(), Convert.ToInt32(ComputerSize));
                
                return View(new LabDetailsViewModel(lab, this));
            }

        }

        private Computer GetComputer(string compName)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Computer> comps = db.Computers.Where(e => e.ComputerName.Equals(compName)).ToList();
                if (comps.Count > 0)
                    return comps.First();
                return null;
            }
        }
    }
}
