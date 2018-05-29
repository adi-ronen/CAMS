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

        public List<LabOccupancyReport> CreateOccupancyLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, List<int> labsIds, bool weekends)
        {
            List<LabOccupancyReport> reports = new List<LabOccupancyReport>();

            foreach (var id in labsIds)
            {
                LabOccupancyReport labReport = CreateOccupancyLabReport(startDate, endDate, startHour, endHour, id, weekends);
                reports.Add(labReport);
            }

            return reports;
        }

        public LabOccupancyReport CreateOccupancyLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, int id, bool weekends)
        {
            Lab lab = _lController.GetLab(id);
            return CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, weekends);

        }

        public LabOccupancyReport CreateOccupancyLabReport(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, Lab lab, bool weekends)
        {
            LabOccupancyReport labReport = new LabOccupancyReport(lab);

            TimeSpan labTotalActiveTime = TimeSpan.Zero;

            List<ComputerLab> cL = getComputerInLab(lab, startDate, endDate);
            List<int> hoursToCheck = new List<int>();
            Dictionary<int, LabHourOccupancyReport> hourReports = new Dictionary<int, LabHourOccupancyReport>();
            Dictionary<DateTime, Dictionary<int, bool>> computerStatusForTime = new Dictionary<DateTime, Dictionary<int, bool>>();
            for (int start = startHour.Hour; start < endHour.Hour; start++)
            {
                hoursToCheck.Add(start);
                hourReports.Add(start, new LabHourOccupancyReport());
            }
            Dictionary<DayOfWeek, LabDayOfWeekReport> weekDayReports = new Dictionary<DayOfWeek, LabDayOfWeekReport>();
            for (int i = 0; i < 7; i++)
            {
                weekDayReports.Add((DayOfWeek)i, new LabDayOfWeekReport());
            }

            //for each day
            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                int maxCompNum = 0;
                int minCompNum = int.MaxValue;
                int sum = 0;
                //for each hour check how many computers were on
                foreach (var hour in hoursToCheck)
                {
                    int computerCount = 0;
                    foreach (var item in cL)
                    {
                        //if the computer were in the lab in this specific day
                        if (date >= item.Entrance && (!item.Exit.HasValue || date <= item.Exit.Value))
                        {
                            if (IsComputerActive(date, hour, item))
                                computerCount++;
                        }
                    }
                    maxCompNum = Math.Max(maxCompNum, computerCount);
                    minCompNum = Math.Min(minCompNum, computerCount);
                    sum += computerCount;
                    hourReports[hour].AddDay(computerCount);
                }
                weekDayReports[date.DayOfWeek].AddDay(maxCompNum, minCompNum, (double)sum / hoursToCheck.Count);

            }
            foreach (var item in weekDayReports)
            {
                labReport.AddByDayReport(item.Value);
            }
            foreach (var item in hourReports)
            {
                labReport.AddByHourReport(item.Value);
            }
            return labReport;
        }

        private static bool IsComputerActive(DateTime date, int hour, ComputerLab cl)
        {
            List<Activity> activities = cl.Computer.Activities.Where(e => e.Mode.Equals(ActivityType.User) && (!e.Login.Date.Equals(date))).ToList();
            //check if the computer had an activity during this hour
            return activities.Where(e => (!((e.Login.Hour > hour + 1) || (e.Logout.Value.Hour < hour)))).Count() > 0;
        }

        private bool IsComputerOn(int computerId,DateTime time)
        {
            return _lController.IsComputerOn(computerId, time);
        }



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

            List<ComputerLab> cL = getComputerInLab(lab, startDate, endDate);
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
        /// <summary>
        /// get all computers that were in the lab during that time or part of it
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<ComputerLab> getComputerInLab(Lab lab, DateTime startDate, DateTime endDate)
        {
            return lab.ComputerLabs.Where(e => (!((e.Entrance > endDate) || (e.Exit < startDate)))).ToList();
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
                && (e.Mode.Equals(ActivityType.User)|| e.Mode.Equals(ActivityType.Class)) // user or class activity
                && !(e.Weekend) 
                && !((e.Login.TimeOfDay >= endHour.TimeOfDay) || (e.Logout.HasValue && e.Logout.Value.TimeOfDay <= startHour.TimeOfDay))).ToList(); //hour range

            }
            else
            {
                compAct = comp.Computer.Activities.Where(e => (e.Login >= newStartDate && e.Logout <= newEndDate) //activities in the report time range
                && (e.Mode.Equals(ActivityType.User) || e.Mode.Equals(ActivityType.Class)) // user or class activity
                && !((e.Login.TimeOfDay >= endHour.TimeOfDay) || (e.Logout.HasValue && e.Logout.Value.TimeOfDay <= startHour.TimeOfDay))).ToList(); //hour range
            }

            compAct = compAct.OrderBy(e => e.Login).ToList();
            DateTime timePointWithClasses = startDate;
            DateTime timePoint = startDate;

            foreach (var act in compAct)
            {

                DateTime endOfActivity = DateTime.Now;
                if (act.Logout.HasValue) endOfActivity = act.Logout.Value;

                DateTime startOfTimeReport = new DateTime(Math.Max(act.Login.Ticks, act.Login.Date.Add(startHour.TimeOfDay).Ticks));
                DateTime enfOfTimeReport = new DateTime(Math.Min(endOfActivity.Ticks,endOfActivity.Date.Add(endHour.TimeOfDay).Ticks));

                //if its user activity add it the activity-time-no-classes
                if (act.Mode.Equals(ActivityType.User))
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
                

            }
            

            // number of hours the computer was in the lab (during the report duration)
            double computerInLabTime = CalculateHoursInReportForComputer(newStartDate, newEndDate, startHour, endHour,weekends);
            ComputerReport cR = new ComputerReport(comp.Computer, computerTotalActiveTime,computerTotalActiveTimeWithClasses, computerInLabTime);
            return cR;
        }

       

        private double CalculateHoursInReportForComputer(DateTime startDate, DateTime endDate, DateTime startHour, DateTime endHour, bool weekends)
        {
            DateTime firstDay = startDate.Date;
            DateTime lastDay = endDate.Date;
            DateTime reportStart = firstDay.Add(startHour.TimeOfDay);
            DateTime reportEnd = firstDay.Add(endHour.TimeOfDay);

            //report starts and ends in the same day
            if (startDate.Date == endDate.Date)
            {
                if (!weekends && (startDate.DayOfWeek == DayOfWeek.Friday || startDate.DayOfWeek == DayOfWeek.Saturday))
                    return 0;
                TimeSpan timeSpan = PeriodIntersectorSpan(startDate, reportStart, endDate, reportEnd);
                return timeSpan.TotalHours;

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

                    DateTime lastReportStart = lastDay.Add(startHour.TimeOfDay);
                    DateTime lastReportEnd = lastDay.Add(endHour.TimeOfDay);
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