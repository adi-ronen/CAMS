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
        ReportModel model;

        public ReportsController()
        {
            model = new ReportModel(this);
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
                startHour = new DateTime();
                endHour = new DateTime().AddDays(1).AddTicks(-1);
            }
            List<LabReport> reports = model.CreateLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds,weekends);

            return View("Details", new LabsReportViewModel(reports,this));
        }

        public ActionResult DisplayLabReportDetails(LabReport report)
        {
            return View("LabDetails", report);
        }


        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(Request.Form["fromDate"]);
                DateTime? endDate = Convert.ToDateTime(Request.Form["toDate"]);
                DateTime? startHour = Convert.ToDateTime(Request.Form["fromTime"]);
                DateTime endHour = Convert.ToDateTime(Request.Form["ToTime"]);
                List<int> labsIds =  Request.Form["LabsIds"].Split(',').Select(int.Parse).ToList();
                bool weekends = Convert.ToBoolean(Request.Form["includeWeekends"]); 
                bool includeAllDay = Convert.ToBoolean(Request.Form["includeAllDay"]);
                if (includeAllDay)
                {
                    startDate = new DateTime();
                    endDate = new DateTime().AddTicks(-1);
                }
                List<LabReport> reports = model.CreateLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);

                return View("Details", new LabsReportViewModel(reports, this));
            }
            catch
            {
                return View(new ReportModel(this));
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

        internal bool IsComputerOn(int computerId, DateTime time)
        {
            var comp = db.Computers.Find(computerId);
            var acts=comp.Activities.Where(e => e.Login <= time && 
                                            ((e.Logout.HasValue && e.Logout.Value >= time)|| (!e.Logout.HasValue))).ToList();
            return (acts.Count > 0);
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
