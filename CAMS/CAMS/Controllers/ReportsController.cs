using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CAMS.Controllers
{
    public class ReportsController : BaseController
    {

        public ReportsController()
        {
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            try
            {
                if (IsViewAccessUser())
                {
                    return View(new ReportModel(this));
                }
                //user have no access to departments
                return RedirectAcordingToLogin();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


        }

        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                ReportModel model = new ReportModel(this);
                DateTime startDate = DateTime.ParseExact(Request.Form["fromDate"], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime? endDate = DateTime.ParseExact(Request.Form["toDate"], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime? startHour = Convert.ToDateTime(Request.Form["fromTime"]);
                DateTime endHour = Convert.ToDateTime(Request.Form["ToTime"]);
                List<int> labsIds = Request.Form["LabsIds"].Split(',').Select(int.Parse).ToList();
                bool weekends = Convert.ToBoolean(Request.Form["includeWeekends"]);
                bool includeAllDay = Convert.ToBoolean(Request.Form["includeAllDay"]);
                string reportType = Request.Form["ReportType"];
                string inclucdeweekends;
                if (weekends) { inclucdeweekends = " כולל סופי שבוע. "; } else { inclucdeweekends = " לא כולל סופי שבוע. "; }
                string title = "מתאריך " + startDate.ToShortDateString() + " עד- " + endDate.Value.ToShortDateString() + inclucdeweekends + "בין השעות " + startHour.Value.ToShortTimeString() + " עד- " + endHour.ToShortTimeString();
                if (includeAllDay)
                {
                    startHour = startDate.Date;
                    endHour = startDate.Date.AddHours(-1);
                }
                if (startHour.Value.Hour >= endHour.Hour)
                    throw new Exception();

                switch (reportType)
                {
                    case "AverageUsage":
                        List<LabReport> LabReport = model.CreateLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("AverageUsage", new LabsReportViewModel(LabReport, this, title));
                    case "LabOccupancyReport-hours":
                        List<LabOccupancyReport> LabOccupancyReport_hours = model.CreateOccupancyLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("LabOccupancyReport", LabOccupancyReport_hours);
                    case "LabOccupancyReport-days":
                        List<LabOccupancyReport> LabOccupancyReport_days = model.CreateOccupancyLabReport(startDate, endDate.Value, startHour.Value, endHour, labsIds, weekends);
                        return View("LabOccupancyReport", LabOccupancyReport_days);
                }
                return View(model);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        internal List<Department> GetUserViewDepartments()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Department> departments = db.Departments.ToList();
                List<Department> userDepartments = new List<Department>();
                foreach (var item in departments)
                {
                    //user can only add labs to the departments he have FULL accsses to.
                    if (IsViewAccess(item.DepartmentId))
                        userDepartments.Add(item);
                }
                return userDepartments;
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



        internal List<Activity> GetComputerUserActivityOnDate(int computerId, DateTime date)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(computerId);
                return comp.Activities.Where(e => e.Mode.Equals(ActivityType.User) && (e.Login.Date.Equals(date))).ToList();
            }

        }
        private Object reportLock = new Object();

        internal List<Activity> GetComputerActivitiesInDateRange(int computerId, DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour)
        {
            lock (reportLock)
            {
                using (var db = new CAMS_DatabaseEntities())
                {
                    Computer comp = db.Computers.Find(computerId);
                    return comp.Activities.Where(e => (e.Login >= startDate && (e.Logout <= endDate || !e.Logout.HasValue)) //activities in the report time range
                                             && (e.Mode.Equals(ActivityType.User) || e.Mode.Equals(ActivityType.Class)) // user or class activity
                                             && !((e.Login.TimeOfDay >= endHour.TimeOfDay) || (e.Logout.HasValue && e.Logout.Value.TimeOfDay <= startHour.TimeOfDay))).ToList(); //hour range;            

                }
            }
        }

        internal Computer GetComputer(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Computers.Find(computerId);
            }

        }
    }
}
