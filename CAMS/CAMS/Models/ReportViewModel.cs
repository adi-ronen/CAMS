using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CAMS.Constant;

namespace CAMS.Models
{
    public class ReportViewModel
    {
        private BaseController _lController;


        public ReportViewModel(BaseController controller)
        {
            this._lController = controller;
        }
        public List<Lab> GetLabsOfDepartment(int DepartmentId)
        {
            return _lController.GetLabsOfDepartment(DepartmentId);
        }
        public List<Department> GetDepartments()
        {
            return _lController.GetDepartments();
        }


        //TBD- class
        public List<LabReport> CreateLabsReport(List<int> labsIds,DateTime startDate,DateTime endDate,DateTime startHour, DateTime endHour)
        {
            List<LabReport> reports = new List<LabReport>();

            foreach (var id in labsIds)
            {
                LabReport labReport = CreateLabReport(startDate, endDate, startHour, endHour, id);
                reports.Add(labReport);
            }



            return reports;
        }

        private LabReport CreateLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, int id)
        {
            Lab lab = _lController.GetLab(id);
            LabReport labReport = new LabReport(lab);

            TimeSpan labTotalActiveTime = TimeSpan.Zero;

            //get all computers that were in the lab during that time or part of it
            List<ComputerLab> cL = lab.ComputerLabs.Where(e => (!((e.Entrance > endDate) || (e.Exit < startDate)))).ToList();
            foreach (var item in cL)
            {
                

                ComputerReport cR = CreateComputerInLabReport(startDate, endDate, startHour, endHour, item);

                //add data to labreport
                labReport.ComputersReport.Add(cR);
                labReport.AddToLabTotalActivityTime(cR.GetComputerTotalActiveTime());
                labReport.AddToLabTotalHours(cR.GetComputerTotalTime());

            }

            return labReport;
        }

        private ComputerReport CreateComputerInLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, ComputerLab comp)
        {
            TimeSpan computerTotalActiveTime = TimeSpan.Zero;
            DateTime compEnterence = comp.Entrance;
            DateTime compExit = DateTime.Now;
            if (comp.Exit.HasValue)
            {
                compExit = comp.Exit.Value;
            }

            DateTime newStartDate = new DateTime(Math.Max(compEnterence.Ticks, startDate.Ticks));
            DateTime newEndDate = new DateTime(Math.Min(compExit.Ticks, endDate.Ticks));
            List<Activity> compAct = comp.Computer.Activities.Where(e => (e.Mode.Equals(ActivityMode.User.ToString())) ||
                //  e.Mode.Equals(ActivityMode.Class.ToString())) &&
                (e.Login >= newStartDate && e.Logout <= newEndDate) && //activities in the report time range
                !(e.Login.Hour > endHour.Hour || (e.Logout.HasValue && e.Logout.Value.Hour < startHour.Hour))).ToList();
            foreach (var act in compAct)
            {

                DateTime copyDate = new DateTime(act.Login.Year, act.Login.Month, act.Login.Day);
                DateTime start2 = copyDate.AddHours(startHour.Hour);
                DateTime end2 = copyDate.AddHours(endHour.Hour);

                computerTotalActiveTime = computerTotalActiveTime.Add(PeriodIntersectorSpan(act.Login, start2, act.Logout.Value, end2));

            }
            // number of hours the computer was in the lab (during the report duration)
            double computerInLabTime = (newEndDate - newStartDate).TotalDays * (endHour - startHour).TotalHours;
            ComputerReport cR = new ComputerReport(comp.Computer, computerTotalActiveTime, computerInLabTime);
            return cR;
        }

        private TimeSpan PeriodIntersectorSpan(DateTime start1,DateTime start2,DateTime end1,DateTime end2)
        {
            return (new DateTime(Math.Min(end1.Ticks, end2.Ticks)) - new DateTime(Math.Max(start1.Ticks, start2.Ticks)));

        }

    }

    

}