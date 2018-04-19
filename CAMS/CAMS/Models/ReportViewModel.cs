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
        private ReportsController _lController;


        public ReportViewModel(ReportsController controller)
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
        public List<LabReport> CreateLabReport(DateTime startDate,DateTime endDate,DateTime startHour, DateTime endHour, List<int> labsIds,bool weekends)
        {
            List<LabReport> reports = new List<LabReport>();

            foreach (var id in labsIds)
            {
                LabReport labReport = CreateLabReport(startDate, endDate, startHour, endHour, id, weekends);
                reports.Add(labReport);
            }



            return reports;
        }

        public LabReport CreateLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, int id, bool weekends)
        {
            Lab lab = _lController.GetLab(id);
            return CreateLabReport(startDate, endDate, startHour, endHour, lab,weekends);
            
        }

     

        public LabReport CreateLabReport (DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, Lab lab, bool weekends)
        {
            LabReport labReport = new LabReport(lab);

            TimeSpan labTotalActiveTime = TimeSpan.Zero;

            //get all computers that were in the lab during that time or part of it
            List<ComputerLab> cL = lab.ComputerLabs.Where(e => (!((e.Entrance > endDate) || (e.Exit < startDate)))).ToList();
            foreach (var item in cL)
            {


                ComputerReport cR = CreateComputerInLabReport(startDate, endDate, startHour, endHour, item,weekends);

                //add data to labreport
                labReport.ComputersReport.Add(cR);
                labReport.AddToLabTotalActivityTime(cR.GetComputerTotalActiveTime());
                labReport.AddToLabTotalHours(cR.GetComputerTotalTime());

            }

            return labReport;
        }

        private ComputerReport CreateComputerInLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, ComputerLab comp, bool weekends)
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
            List<Activity> compAct = comp.Computer.Activities.Where(e => (e.Mode.Equals(ActivityMode.User.ToString())) 
                //  || e.Mode.Equals(ActivityMode.Class.ToString())) 
                && (e.Login >= newStartDate && e.Logout <= newEndDate) && //activities in the report time range
                !((e.Login.Hour > endHour.Hour) || (e.Logout.HasValue && e.Logout.Value.Hour < startHour.Hour))).ToList();
            foreach (var act in compAct)
            {
                if (!weekends && (act.Login.DayOfWeek == DayOfWeek.Friday || act.Login.DayOfWeek == DayOfWeek.Saturday))
                {
                    continue;
                }
                computerTotalActiveTime= computerTotalActiveTime.Add(ActivityTimeInReport(startHour, endHour, act));

            }
            // number of hours the computer was in the lab (during the report duration)
            double computerInLabTime = CalculateHoursInReportForComputer(newStartDate, newEndDate, startHour, endHour,weekends);
            ComputerReport cR = new ComputerReport(comp.Computer, computerTotalActiveTime, computerInLabTime);
            return cR;
        }

        private TimeSpan ActivityTimeInReport(DateTime startHour, DateTime endHour, Activity act)
        {
            DateTime copyDate = new DateTime(act.Login.Year, act.Login.Month, act.Login.Day);
            DateTime start2 = copyDate.AddHours(startHour.Hour);
            DateTime end2 = copyDate.AddHours(endHour.Hour);

            return PeriodIntersectorSpan(act.Login, start2, act.Logout.Value, end2);
        }

        private double CalculateHoursInReportForComputer(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, bool weekends)
        {
            DateTime firstDay = startDate.Date;
            DateTime lastDay = endDate.Date;
            DateTime reportStart = firstDay.AddHours(startHour.Hour);
            DateTime reportEnd = firstDay.AddHours(endHour.Hour);

            //report starts and ends in the same day
            if (startDate.Date == endDate.Date)
            {
                if (!weekends && (startDate.DayOfWeek == DayOfWeek.Friday || startDate.DayOfWeek == DayOfWeek.Saturday))
                    return 0;
                TimeSpan timeSpan = PeriodIntersectorSpan(startDate, reportStart, endDate, reportEnd);
                return timeSpan.Hours;

            }
            else
            {
                TimeSpan timeSpanFirstDay = TimeSpan.Zero, timeSpanLastDay = TimeSpan.Zero;
                if (weekends || (startDate.DayOfWeek != DayOfWeek.Friday && startDate.DayOfWeek != DayOfWeek.Saturday))
                {
                    //hours in report in the first day
                    DateTime startDayEnd = firstDay.AddDays(1);
                    timeSpanFirstDay = PeriodIntersectorSpan(startDate, reportStart, startDayEnd, reportEnd);
                }
                if (weekends || (endDate.DayOfWeek != DayOfWeek.Friday && endDate.DayOfWeek != DayOfWeek.Saturday))
                {
                    //hours in report in the last day

                    DateTime lastReportStart = lastDay.AddHours(startHour.Hour);
                    DateTime lastReportEnd = lastDay.AddHours(endHour.Hour);
                    timeSpanLastDay = PeriodIntersectorSpan(lastDay, lastReportStart, endDate, lastReportEnd);
                }
                //number of days between
                double days = (lastDay - firstDay.AddDays(1)).TotalDays;
                if (!weekends&&days>0)
                    days = ColculateBusinessDays(lastDay.AddDays(-1), firstDay.AddDays(1));
                return (days * (endHour - startHour).TotalHours) + timeSpanFirstDay.Add(timeSpanLastDay).TotalHours;

            }

        }

        public double ColculateBusinessDays(DateTime endD, DateTime startD)
        {
            int totalDays = 1+(int)(endD - startD).TotalDays;
            int businessDays = (totalDays / 7) * 5;
            int residual = (totalDays) % 7;
            int days = (int)startD.DayOfWeek + residual - 1;
            if(startD.DayOfWeek == DayOfWeek.Saturday || days == 5)
            {
                residual--;
            }
            else if (days>5)
            {
                residual -= 2;

            }
            return businessDays + residual;

           
        }

        private TimeSpan PeriodIntersectorSpan(DateTime start1,DateTime start2,DateTime end1,DateTime end2)
        {

            if(Math.Min(end1.Ticks, end2.Ticks)<Math.Max(start1.Ticks, start2.Ticks)){
                return TimeSpan.Zero;
            }
            return (new DateTime(Math.Min(end1.Ticks, end2.Ticks)) - new DateTime(Math.Max(start1.Ticks, start2.Ticks)));

        }

    }

    

}