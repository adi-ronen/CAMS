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

        public string[] GetHours()
        {
            string[] hours = new string[ByHours.Count];
            int i = 0;
            foreach (var item in ByHours)
            {
                int endTime = item.Hour + 1;
                hours[i] = item.Hour + ":00-" + endTime + ":00";
                i++;
            }
            return hours;
        }
        public string[] GetHoursMax()
        {
            string[] max = new string[ByHours.Count];
            int i = 0;
            foreach (var item in ByHours)
            {
                max[i] = String.Format("{0:P2}.", item.MaxOccupancy).Replace("%","").Trim();
                i++;
            }
            return max;
        }
        public string[] GetHoursMin()
        {
            string[] min = new string[ByHours.Count];
            int i = 0;
            foreach (var item in ByHours)
            {
                min[i] = String.Format("{0:P2}.", item.MinOccupancy).Replace("%", "").Trim();
                i++;
            }
            return min;
        }
        public string[] GetHoursAvg()
        {
            string[] avg = new string[ByHours.Count];
            int i = 0;
            foreach (var item in ByHours)
            {
                avg[i] = String.Format("{0:P2}.", item.AvgOccupancy).Replace("%", "").Trim();
                i++;
            }
            return avg;
        }

        public string[] GetDaysOfWeek()
        {
            string[] days = new string[ByDay.Count];
            int i = 0;
            foreach (var item in ByDay)
            {
                days[i] = item.DayOfWeek.ToString();
                i++;
            }
            return days;
        }
        public string[] GetDaysMax()
        {
            string[] max = new string[ByDay.Count];
            int i = 0;
            foreach (var item in ByDay)
            {
                max[i] = String.Format("{0:P2}.", item.MaxOccupancy);
                i++;
            }
            return max;
        }
        public string[] GetDaysMin()
        {
            string[] min = new string[ByDay.Count];
            int i = 0;
            foreach (var item in ByHours)
            {
                min[i] = String.Format("{0:P2}.", item.MinOccupancy);
                i++;
            }
            return min;
        }
        public string[] GetDaysAvg()
        {
            string[] avg = new string[ByDay.Count];
            int i = 0;
            foreach (var item in ByDay)
            {
                avg[i] = String.Format("{0:P2}.", item.AvgOccupancy);
                i++;
            }
            return avg;
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