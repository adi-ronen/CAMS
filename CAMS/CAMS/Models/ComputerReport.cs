using System;

namespace CAMS.Models
{
    public class ComputerReport
    {
        public Computer Computer;
        public double AverageUsage
        {
            get
            {
                if (computerTotaHours == 0) return 0;
                return (computerTotalActiveTime.TotalHours / computerTotaHours) * 100;
            }
        }
        public double ScheduleAverageUsage
        {
            get
            {
                if (computerTotaHours == 0) return 0;
                return (computerTotalActiveTimeWithClasses.TotalHours / computerTotaHours) * 100;
            }
        }
        private TimeSpan computerTotalActiveTime;
        private TimeSpan computerTotalActiveTimeWithClasses;
        private double computerTotaHours;


        public ComputerReport(Computer computer, TimeSpan _computerTotalActiveTime, TimeSpan _computerTotalActiveTimeClasses, double _computerTotaHours)
        {
            Computer = computer;
            computerTotalActiveTime = _computerTotalActiveTime;
            computerTotalActiveTimeWithClasses = _computerTotalActiveTimeClasses;
            computerTotaHours = _computerTotaHours;
        }

        public TimeSpan GetComputerTotalActiveTime()
        {
            return computerTotalActiveTime;
        }
        
        public double GetComputerTotalTime()
        {
            return computerTotaHours;
        }

        internal TimeSpan GetComputerTotalActiveTimeWithClasses()
        {
            return computerTotalActiveTimeWithClasses;
        }
    }
}