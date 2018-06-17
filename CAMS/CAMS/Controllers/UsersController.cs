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
    public class UsersController : Controller
    {
        private CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        // GET: Users
        public ActionResult Index(bool? byDepartment)
        {
            if (byDepartment.HasValue)
                ViewBag.byDepartment = byDepartment.Value;
            else
                ViewBag.byDepartment = false;
            User user = db.Users.Find(1);
            return View(new AccessViewModel(user,this, ViewBag.byDepartment));
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        internal List<SelectListItem> GetUsers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            List<User> users = db.Users.ToList();
            foreach (User u in users)
            {
                list.Add(new SelectListItem { Text = u.Email });
            }

            return list;
        }

        internal List<SelectListItem> GetDepartmentsList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            List<Department> departments = db.Departments.ToList();
            foreach (Department d in departments)
            {
                list.Add(new SelectListItem { Text = d.DepartmentName, Value = d.DepartmentId.ToString() });
            }

            return list;
        }

        internal void AddUser(string v)
        {
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            User user = db.Users.Find(1);
            return View(new AccessViewModel(user, this));
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string user_id = Request.Form["UsersList"];
                if (user_id == string.Empty)
                {
                    AddUser(user_id);
                }
                UserDepartment userDepartment = new UserDepartment
                {
                    UserId = Convert.ToInt32(Request.Form["UsersList"].ToString()),
                    DepartmentId = Convert.ToInt32(Request.Form["Departments"].ToString()),
                    AccessType = (AccessType)Convert.ToByte(Request.Form["AccessType"].ToString())
                };
                db.UserDepartments.Add(userDepartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                User user = db.Users.Find(1);
                return View(new AccessViewModel(user, this));
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? userId, int? depId)
        {
            if (userId == null || depId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDepartment userDep = db.UserDepartments.Where(x => x.DepartmentId == depId && x.UserId == userId).ToList().First();
            if (userDep == null)
            {
                return HttpNotFound();
            }
            return View(userDep);
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "UserId, DepartmentId, AccessType")] UserDepartment UserDepartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(UserDepartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(UserDepartment);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? userId, int? depId)
        {
            if (userId == null || depId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDepartment userDep = db.UserDepartments.Where(x => x.DepartmentId == depId && x.UserId == userId).ToList().First();
            if (userDep == null)
            {
                return HttpNotFound();
            }
            return View(userDep);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? userId, int? depId)
        {
            UserDepartment userDep = db.UserDepartments.Where(x => x.DepartmentId == depId && x.UserId == userId).ToList().First();
            db.UserDepartments.Remove(userDep);
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
