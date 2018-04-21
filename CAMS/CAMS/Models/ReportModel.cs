using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CAMS.Constant;

namespace CAMS.Models
{
    public class ReportModel
    {
        private ReportsController _lController;


        public ReportModel(ReportsController controller)
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
                labReport.AddToLabTotalActivityTimeWithClasses(cR.GetComputerTotalActiveTimeWithClasses());
                labReport.AddToLabTotalHours(cR.GetComputerTotalTime());

            }

            return labReport;
        }

        private ComputerReport CreateComputerInLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, ComputerLab comp, bool weekends)
        {
            TimeSpan computerTotalActiveTime = TimeSpan.Zero;
            TimeSpan computerTotalActiveTimeWithClasses = TimeSpan.Zero;

            DateTime compEnterence = comp.Entrance;
            DateTime compExit = DateTime.Now;
            if (comp.Exit.HasValue)
            {
                compExit = comp.Exit.Value;
            }

            DateTime newStartDate = new DateTime(Math.Max(compEnterence.Ticks, startDate.Ticks));

            List<Activity> compAct;
            DateTime newEndDate = new DateTime(Math.Min(compExit.Ticks, endDate.Ticks));
            if (!weekends)
            {
                compAct = comp.Computer.Activities.Where(e => (e.Login >= newStartDate && e.Logout <= newEndDate) //activities in the report time range
                && (e.Mode.Trim().Equals(ActivityMode.User.ToString())|| e.Mode.Trim().Equals(ActivityMode.Class.ToString())) // user or class activity
                && !(e.Weekend) 
                && !((e.Login.Hour > endHour.Hour) || (e.Logout.HasValue && e.Logout.Value.Hour < startHour.Hour))).ToList(); //hour range

            }
            else
            {
                compAct = comp.Computer.Activities.Where(e => (e.Login >= newStartDate && e.Logout <= newEndDate) //activities in the report time range
                && (e.Mode.Trim().Equals(ActivityMode.User.ToString()) || e.Mode.Trim().Equals(ActivityMode.Class.ToString())) // user or class activity
                && !((e.Login.Hour > endHour.Hour) || (e.Logout.HasValue && e.Logout.Value.Hour < startHour.Hour))).ToList(); //hour range
            }

            compAct = compAct.OrderBy(e => e.Login).ToList();
            DateTime timePointWithClasses = startDate;
            DateTime timePoint = startDate;

            foreach (var act in compAct)
            {

                DateTime endOfActivity = DateTime.Now;
                if (act.Logout.HasValue) endOfActivity = act.Logout.Value;

                DateTime startOfTimeReport = new DateTime(Math.Max(act.Login.Ticks, act.Login.Date.AddHours(startHour.Hour).Ticks));
                DateTime enfOfTimeReport = new DateTime(Math.Min(endOfActivity.Ticks,endOfActivity.Date.AddHours(endHour.Hour).Ticks));

                //if its user activity add it the activity-time-no-classes
                if (act.Mode.Trim().Equals(ActivityMode.User.ToString()))
                {
                    TimeSpan timeToAdd = enfOfTimeReport - startOfTimeReport;
                    computerTotalActiveTime = computerTotalActiveTime.Add(timeToAdd);
                    timePoint = enfOfTimeReport;
                }
                //the time point is before the end of the activity there is time to add
                if (enfOfTimeReport > timePointWithClasses)
                {
                    // strart from the latest point out of the two (start point and time point)
                    startOfTimeReport = new DateTime(Math.Max(startOfTimeReport.Ticks,timePointWithClasses.Ticks));
                    TimeSpan timeToAdd = enfOfTimeReport - startOfTimeReport;
                    //add to activity-time-with-classes (for both user and cass activity)
                    computerTotalActiveTimeWithClasses = computerTotalActiveTimeWithClasses.Add(timeToAdd);
                    timePointWithClasses = enfOfTimeReport;
                }

                


                ////if the time-point is after the end of the activity- we already counted that time in active-time-incuding-classes
                //bool unincludedActiveTime = !act.Logout.HasValue || act.Logout.Value > timePoint;
                //bool isClassActivity = act.Mode.Trim().Equals(ActivityMode.Class);
                ////if it is a class activity and we already included its activity time
                //if (isClassActivity && !unincludedActiveTime)
                //    continue;
                ////if user activity include it in the active-time-not-including-classes 
                //if (!isClassActivity)
                //{
                //    TimeSpan timeToAdd = ActivityTimeInReport(startHour, endHour, act);
                //    computerTotalActiveTime = computerTotalActiveTime.Add(timeToAdd);
                //    //if its not yet included in the active-time-incuding-classes and same timespan
                //    if (unincludedActiveTime && timePoint < act.Login)
                //    {
                //        computerTotalActiveTimeWithClasses = computerTotalActiveTimeWithClasses.Add(timeToAdd);
                //        if (act.Logout.HasValue) timePoint = act.Logout.Value;
                //        else timePoint = endDate;
                //        continue;
                //    }

                //}
                ////if its not yet included in the active-time-incuding-classes
                //if (unincludedActiveTime)
                //{
                    
                //    TimeSpan timeToAdd = ActivityTimeInReport(startHour, endHour, timePoint,act.Logout);
                //    computerTotalActiveTimeWithClasses = computerTotalActiveTimeWithClasses.Add(timeToAdd);
                //    if (act.Logout.HasValue) timePoint = act.Logout.Value;
                //    else timePoint = endDate;
                //}

            }
            

            // number of hours the computer was in the lab (during the report duration)
            double computerInLabTime = CalculateHoursInReportForComputer(newStartDate, newEndDate, startHour, endHour,weekends);
            ComputerReport cR = new ComputerReport(comp.Computer, computerTotalActiveTime,computerTotalActiveTimeWithClasses, computerInLabTime);
            return cR;
        }

        //private TimeSpan ActivityTimeInReport(DateTime startHour, DateTime endHour, DateTime startPoint, DateTime? end)
        //{
        //    DateTime endPoint = DateTime.Now;
        //    if (end.HasValue)
        //        endPoint = end.Value;
        //    DateTime copyDate = startPoint.Date;
        //    DateTime start2 = copyDate.AddHours(startHour.Hour);
        //    DateTime end2 = copyDate.AddHours(endHour.Hour);

        //    return PeriodIntersectorSpan(startPoint, start2, endPoint, end2);
        //}

        //private TimeSpan ActivityTimeInReport(DateTime startHour, DateTime endHour, Activity act)
        //{
        //    return ActivityTimeInReport(startHour, endHour, act.Login, act.Logout);
        //}

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
            TimeSpan time = new DateTime(Math.Min(end1.Ticks, end2.Ticks)) - new DateTime(Math.Max(start1.Ticks, start2.Ticks));
            if (time.Ticks<0)
            {
                return TimeSpan.Zero;
            }
            return time;

        }

    }

    

}