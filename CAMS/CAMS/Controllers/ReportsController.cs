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

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View(model);
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
                string reportType = Request.Form["ReportType"];
                string inclucdeweekends;
                if (weekends){inclucdeweekends = " כולל סופי שבוע. ";}else{inclucdeweekends = " לא כולל סופי שבוע. ";}
                string title = "מתאריך " + startDate.ToShortDateString() + " עד- " + endDate.Value.ToShortDateString() + inclucdeweekends + "בין השעות " + startHour.Value.ToShortTimeString() + " עד- " + endHour.ToShortTimeString();
                if (includeAllDay)
                {
                    startDate = new DateTime();
                    endDate = new DateTime().AddTicks(-1);
                }
                switch (reportType)
                {
                    case "AverageUsage":
                        List<LabReport> LabReport = model.CreateLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("AverageUsage", new LabsReportViewModel(LabReport, this, title));
                    case "LabOccupancyReport_hours":
                        List<LabOccupancyReport> LabOccupancyReport_hours = model.CreateOccupancyLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("LabOccupancyReport", LabOccupancyReport_hours);
                    case "LabOccupancyReport_days":
                        List<LabOccupancyReport> LabOccupancyReport_days = model.CreateOccupancyLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("LabOccupancyReport", LabOccupancyReport_days);
                }
                return View(new ReportModel(this));
            }
            catch
            {
                return View(new ReportModel(this));
            }
        }
        internal bool IsComputerOn(int computerId, DateTime time)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                var comp = db.Computers.Find(computerId);
                var acts = comp.Activities.Where(e => e.Login <= time &&
                                                  ((e.Logout.HasValue && e.Logout.Value >= time) || (!e.Logout.HasValue))).ToList();
                return (acts.Count > 0);
            }
        }
    }
}
