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
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Index(bool? byDepartment)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (byDepartment.HasValue)
                    ViewBag.byDepartment = byDepartment.Value;
                else
                    ViewBag.byDepartment = false;
                User user = db.Users.Find(2);
                if (user == null)
                {
                    return HttpNotFound();
                }
                db.Entry(user).Collection(e => e.UserDepartments).Load();
                return View(new AccessViewModel(user, this, ViewBag.byDepartment));
            }
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                db.Entry(user).Collection(e => e.UserDepartments).Load();
                return View(user);
            }
        }

        internal List<SelectListItem> GetUsers()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<User> users = db.Users.ToList();
                foreach (User u in users)
                {
                    list.Add(new SelectListItem { Text = u.Email, Value = u.UserId.ToString() });
                }

                return list;
            }
        }

        internal List<SelectListItem> GetDepartmentsList()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Department> departments = db.Departments.ToList();
                foreach (Department d in departments)
                {
                    list.Add(new SelectListItem { Text = d.DepartmentName, Value = d.DepartmentId.ToString() });
                }

                return list;
            }
        }

        internal User AddUser(string email)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<User> userl = db.Users.Where(e => e.Email.Equals(email)).ToList();
                if (userl.Count > 0)
                {
                    return userl.First();
                }
                User u = new User
                {
                    Email = email
                };
                db.Users.Add(u);
                return u;
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                User user = db.Users.Find(2);
                if (user == null)
                {
                    return HttpNotFound();
                }
                db.Entry(user).Collection(e => e.UserDepartments).Load();
                return View(new AccessViewModel(user, this));
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                try
                {
                    string user_email = Request.Form["UsersList"];
                    User user;
                    List<User> userl = db.Users.Where(e => e.Email.Equals(user_email)).ToList();
                    if (userl.Count > 0)
                    {
                        user = userl.First();
                    }
                    else
                    {
                        user=AddUser(user_email);

                    }
                    string ll = Request.Form["Departments"].ToString();
                    int dsl = Convert.ToInt32(Request.Form["Departments"].ToString());
                    AccessType jld = (AccessType)Convert.ToByte(Request.Form["AccessType"].ToString());

                    UserDepartment userDepartment = new UserDepartment
                    {
                        UserId = user.UserId,
                        DepartmentId = Convert.ToInt32(Request.Form["Departments"].ToString()),
                        AccessType = (AccessType)Convert.ToByte(Request.Form["AccessType"].ToString())
                    };
                    db.UserDepartments.Add(userDepartment);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    User user = db.Users.Find(2);
                    db.Entry(user).Collection(e => e.UserDepartments).Load();
                    return View(new AccessViewModel(user, this));
                }
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? userId, int? depId)
        {
            if (userId == null || depId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                UserDepartment userDep = db.UserDepartments.Where(x => x.DepartmentId == depId && x.UserId == userId).ToList().First();
                if (userDep == null)
                {
                    return HttpNotFound();
                }
                db.Entry(userDep).Reference(e => e.User).Load();
                db.Entry(userDep).Reference(e => e.Department).Load();
                return View(userDep);
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "UserId, DepartmentId, AccessType")] UserDepartment UserDepartment)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(UserDepartment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(UserDepartment);
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? userId, int? depId)
        {
            using (var db = new CAMS_DatabaseEntities())
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
                db.Entry(userDep).Reference(e => e.User).Load();
                db.Entry(userDep).Reference(e => e.Department).Load();
                return View(userDep);
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? userId, int? depId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                UserDepartment userDep = db.UserDepartments.Where(x => x.DepartmentId == depId && x.UserId == userId).ToList().First();
                db.UserDepartments.Remove(userDep);
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
    }
}
