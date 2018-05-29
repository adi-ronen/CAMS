using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class LabOccupancyReport
    {
        public Lab Lab;
        public List<LabHourOccupancyReport> ByHours;
        public List<LabDayOfWeekReport> ByDay;

        public LabOccupancyReport(Lab lab)
        {
            Lab = lab;
            ByHours = new List<LabHourOccupancyReport>();
            ByDay = new List<LabDayOfWeekReport>();
        }
        public void AddByHourReport(LabHourOccupancyReport report)
        {
            ByHours.Add(report);
        }
        public void AddByDayReport(LabDayOfWeekReport report)
        {
            ByDay.Add(report);
        }

    }


    public class LabHourOccupancyReport
    {
        public int Hour;
        public double MaxOccupancy
        {
        get {
                if (computersinUseEachDay.Count > 0)
                    return computersinUseEachDay.Max();
                return 0;
            }
        }
        public double MinOccupancy
        {
            get
            {
                if (computersinUseEachDay.Count > 0)
                    return computersinUseEachDay.Min();
                return 0;
            }
        }
        public double AvgOccupancy
        {
            get
            {
                if (computersinUseEachDay.Count > 0)
                    return computersinUseEachDay.Average();
                return 0;
            }
        }
        private List<double> computersinUseEachDay = new List<double>();
        public LabHourOccupancyReport(int hour)
        {
            Hour = hour;
        }

        public void AddDay(double compNum)
        {
            computersinUseEachDay.Add(compNum);
        }


    }
    public class LabDayOfWeekReport
    {
        
        public DayOfWeek DayOfWeek;
        public double MaxOccupancy
        {
            get
            {
                if(maxComputersInUse.Count>0)
                    return maxComputersInUse.Max();
                return 0;
            }
        }
        public double MinOccupancy
        {
            get
            {
                if (minComputersInUse.Count > 0)
                    return minComputersInUse.Min();
                return 0;
            }
        }
        public double AvgOccupancy
        {
            get
            {
                if (avgComputerinUse.Count > 0)
                    return avgComputerinUse.Average();
                return 0;
            }
        }
        private List<double> maxComputersInUse = new List<double>();
        private List<double> minComputersInUse = new List<double>();
        private List<double> avgComputerinUse = new List<double>();

        public LabDayOfWeekReport(DayOfWeek day)
        {
            DayOfWeek = day;
        }

        public void AddDay(double maxCompNum, double minCompNum,double avgCompNum)
        {
            maxComputersInUse.Add(maxCompNum);
            minComputersInUse.Add(minCompNum);
            avgComputerinUse.Add(avgCompNum);

        }
    }
}