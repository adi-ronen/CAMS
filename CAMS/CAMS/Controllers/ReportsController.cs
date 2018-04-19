using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAMS.Controllers
{
    public class ReportsController : BaseController
    {
        ReportViewModel model;

        public ReportsController()
        {
            model = new ReportViewModel(this);
        }

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View(model);
        }
        public ActionResult CreateReport(DateTime startDate, DateTime? endDate, DateTime? startHour, DateTime endHour, List<int> labsIds, bool weekends, bool allDay)
        {
            if (allDay)
            {
                startDate = new DateTime();
                endDate = new DateTime().AddTicks(-1);
            }
            List<LabReport> reports = model.CreateLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds,weekends);

            return View(reports);
        }
        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
    }
}
