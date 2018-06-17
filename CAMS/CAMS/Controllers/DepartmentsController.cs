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
    public class DepartmentsController : BaseController
    {

        // GET: Departments
        public ActionResult Index()
        {
            if (GetConnectedUser()!=-1)
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    return View(db.Departments.ToList());
                }
            }
            else
                return RedirectToAction("Login", "Account");

        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (GetConnectedUser() != -1)
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    Department department = db.Departments.Find(id);
                    if (department == null || !IsFullAccess(department.DepartmentId))
                    {
                        return HttpNotFound();
                    }
                    return View(department);

                }
            }
            else
                return RedirectToAction("Login", "Account");
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentName,Domain")] Department department)
        {
            if (GetConnectedUser() != -1)
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    if (ModelState.IsValid)
                    {
                        db.Departments.Add(department);
                        db.SaveChanges();
                        UserDepartment userDepartment = new UserDepartment
                        {
                            UserId = GetConnectedUser(),
                            DepartmentId = department.DepartmentId,
                            AccessType = AccessType.Full
                        };
                        db.UserDepartments.Add(userDepartment);
                        db.SaveChanges();
                        ((Dictionary<int, AccessType>)Session["Accesses"]).Add(userDepartment.DepartmentId, userDepartment.AccessType);
                        return RedirectToAction("Index");
                    }

                    return View(department);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");

            }

        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (GetConnectedUser() != -1)
                {
                    Department department = db.Departments.Find(id);
                    if (department == null || !IsFullAccess(department.DepartmentId))
                    {
                        return HttpNotFound();
                    }
                    return View(department);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName,Domain")] Department department)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(department);
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (GetConnectedUser() != -1)
                {
                    Department department = db.Departments.Find(id);
                    if (department == null || !IsFullAccess(department.DepartmentId))
                    {
                        return HttpNotFound();
                    }
                    return View(department);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Department department = db.Departments.Find(id);
                List<int> labsId = department.Labs.Select(e => e.LabId).ToList();
                foreach (var lbid in labsId)
                {
                    DeleteLab(lbid);
                }
                db.Departments.Remove(department);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return DeleteConfirmed(id);
                }
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
    }
}
