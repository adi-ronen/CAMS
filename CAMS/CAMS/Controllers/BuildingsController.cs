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
    public class BuildingsController : BaseController
    {

        // GET: Buildings
        public ActionResult Index()
        {
            return View(db.Labs.Select(e => e.Building).Distinct().ToList());
        }
        

        // GET: Buildings/Edit/building name
        public ActionResult Edit(string building)
        {
            if (building == null || building == string.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Department department = db.Departments.Find(id);
            //if (department == null)
            //{
            //    return HttpNotFound();
            //}
            object buildingName = building;
            return View(buildingName);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string NewName, string OldName)
        {
            if (NewName == null|| NewName == string.Empty || OldName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(NewName);
        }

        // GET: Buildings/Delete/name
        public ActionResult Delete(string id)
        {
            return DeleteConfirmed(id);
        }

        // POST: Buildings/Delete/name
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            List<int> labsId = db.Labs.Where(e => e.Building.Equals(id)).Select(e => e.LabId).ToList();
            foreach (var lbid in labsId)
            {
                DeleteLab(lbid);
            }
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
