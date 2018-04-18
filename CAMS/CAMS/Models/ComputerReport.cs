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
                return 0;
            }
        }
        private TimeSpan computerTotalActiveTime;
        private double computerTotaHours;


        public ComputerReport(Computer computer, TimeSpan _computerTotalActiveTime, double _computerTotaHours)
        {
            Computer = computer;
            computerTotalActiveTime = _computerTotalActiveTime;
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

        

    }
}