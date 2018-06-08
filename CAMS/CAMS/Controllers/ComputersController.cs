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
    public class ComputersController : BaseController
    {
        // GET: Computers
        public ActionResult Index()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                var computers = db.Computers.Include(c => c.Lab);
                return View(computers.ToList());
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    return HttpNotFound();
                }

                return View(computer);
            }
        }




        // GET: Computers/Create
        public ActionResult Create()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                ViewBag.CurrentLab = new SelectList(db.Labs, "LabId");
                return View();
            }
        }



        public void AddCompToLab(string compName, int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);
                //while( lab.ComputerLabs.Count>0)
                //{
                //    db.ComputerLabs.Remove(lab.ComputerLabs.First());

                //}
                //while (lab.Computers.Count > 0)
                //{
                //    db.Computers.Remove(lab.Computers.First());

                //}
                //db.SaveChanges();
                Computer comp = new Computer();
                comp.ComputerName = compName;
                comp.CurrentLab = labId;
                comp.LocationInLab = "0%,0%";
                comp.ComputerId = db.Computers.Max(e => e.ComputerId) + 1;
                db.Computers.Add(comp);
                db.SaveChanges();

                ComputerLab cl = new ComputerLab();

                cl.ComputerId = comp.ComputerId;
                cl.LabId = labId;
                cl.Entrance = DateTime.Now.Date;
                db.ComputerLabs.Add(cl);
                db.SaveChanges();
            }
        }


        // POST: Computers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComputerId,MAC,ComputerName,CurrentLab,LocationInLab")] Computer computer)
        {
            using (var db = new CAMS_DatabaseEntities())
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
        }



        // GET: Computers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CurrentLab = new SelectList(db.Labs, "LabId", "LabName", computer.CurrentLab);
                return View(computer);
            }
        }

        // POST: Computers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComputerId,MAC,ComputerName,CurrentLab,LocationInLab")] Computer computer)
        {
            using (var db = new CAMS_DatabaseEntities())
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
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    return HttpNotFound();
                }
                return View(computer);
            }
        }

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer computer = db.Computers.Find(id);
                db.Computers.Remove(computer);
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
