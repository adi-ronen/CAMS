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

    }


    public class LabHourOccupancyReport
    {
        public int Hour;
        public int MaxOccupancy
        {
        get {
                return computersinUseEachDay.Max();
            }
        }
        int MinOccupancy
        {
            get
            {
                return computersinUseEachDay.Min();
            }
        }
        double AvgOccupancy
        {
            get
            {
                return computersinUseEachDay.Average();
            }
        }
        private List<int> computersinUseEachDay = new List<int>();

        public void AddDay(int compNum)
        {
            computersinUseEachDay.Add(compNum);
        }


    }
    public class LabDayOfWeekReport
    {
        DayOfWeek DayOfWeek;
        public int MaxOccupancy
        {
            get
            {
                return maxComputersInUse.Max();
            }
        }
        int MinOccupancy
        {
            get
            {
                return minComputersInUse.Min();
            }
        }
        double AvgOccupancy
        {
            get
            {
                return avgComputerinUse.Average();
            }
        }
        private List<int> maxComputersInUse = new List<int>();
        private List<int> minComputersInUse = new List<int>();
        private List<double> avgComputerinUse = new List<double>();


        public void AddDay(int maxCompNum,int minCompNum,double avgCompNum)
        {
            maxComputersInUse.Add(maxCompNum);
            minComputersInUse.Add(minCompNum);
            avgComputerinUse.Add(avgCompNum);

        }
    }
}