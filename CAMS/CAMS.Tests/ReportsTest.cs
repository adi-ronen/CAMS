using System;
using System.Collections.Generic;
using CAMS.Controllers;
using CAMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CAMS.Tests
{
    [TestClass]
    public class ReportsTest
    {

        ReportsController controller = new ReportsController();
        

        [TestInitialize]
        public void Initialize()
        {
           

        }
        [TestMethod]
        public void TestReportWithoutWeekends()
        {
            Lab lab = new Lab();
            lab.LabId = 3000;
            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2018, 4, 15);
            cl.Exit = new DateTime(2018, 5, 15);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);

            Activity act1 = new Activity();
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2018, 4, 16, 10, 0, 0);
            act1.Logout = new DateTime(2018, 4, 16, 12, 0, 0);
            act1.Mode = ActivityType.User;

            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2018, 4, 16);
            DateTime endDate = new DateTime(2018, 4, 17);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;


            //
            string msg = "one 2 hour activity";
            LabReport lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");
            //
            msg = "one 2 hour activity no weekends";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, false);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");

           
            
            // two activities (16/4)10-12, (20/4)13-15
            Activity act2 = new Activity();
            act2.Computer = cl.Computer;
            cl.Computer.Activities.Add(act2);
            act2.Login = new DateTime(2018, 4, 20, 13, 0, 0);
            act2.Logout = new DateTime(2018, 4, 20, 15, 0, 0);
            act2.Mode = ActivityType.User;
            act2.Weekend = true;

            //
            msg = "activity in weekend-report include weekend";
            startDate = new DateTime(2018, 4, 20);
            endDate = new DateTime(2018, 4, 21);
            startHour = new DateTime().Date + new TimeSpan(08, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");
            //
            msg = "activity in weekend-day report disclude weekend";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, false);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(0, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");
            
            //
            msg = "activity in weekend-week report include weekend";
            startDate = new DateTime(2018, 4, 12);
            endDate = new DateTime(2018, 4, 21);
            startHour = new DateTime().Date + new TimeSpan(08, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(4, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(60, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(((double)4/60)*100, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(((double)4 / 60) * 100, lr.AverageUsage, msg + "- avarageUsage of lab ");
            //
            msg = "activity in weekend-week report disclude weekend";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, false);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(50, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(4, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(4, lr.AverageUsage, msg + "- avarageUsage of lab ");


        }
        
        [TestMethod]
        public void TestColculateBusinessDaysFunc()
        {
            ReportModel model = new ReportModel(controller);
            DateTime start = new DateTime(2018, 04, 19);
            DateTime end = new DateTime(2018, 04, 26);
            Assert.AreEqual(6, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 04, 20);
            end = new DateTime(2018, 04, 25);
            Assert.AreEqual(4, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 04, 21);
            end = new DateTime(2018, 04, 25);
            Assert.AreEqual(4, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 03, 1);
            end = new DateTime(2018, 03, 31);
            Assert.AreEqual(21, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 2, 1);
            end = new DateTime(2018, 2, 28);
            Assert.AreEqual(20, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 1, 1);
            end = new DateTime(2018, 1, 31);
            Assert.AreEqual(23, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 1, 6);
            end = new DateTime(2018, 1,6);
            Assert.AreEqual(0, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 1, 6);
            end = new DateTime(2018, 1, 13);
            Assert.AreEqual(5, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 1, 5);
            end = new DateTime(2018, 1, 6);
            Assert.AreEqual(0, model.ColculateBusinessDays(end, start));
            start = new DateTime(2018, 1, 17);
            end = new DateTime(2018, 1, 31);
            Assert.AreEqual(11, model.ColculateBusinessDays(end, start));
        }


        [TestMethod]
        public void TestComputerEnterAndExitDuringReport()
        {

            Lab lab = new Lab();
            lab.LabId = 3000;
            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2010, 10, 10, 10, 0, 0);
            cl.Exit = new DateTime(2010, 10, 15, 12, 0, 0);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);


            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2010, 10, 9);
            DateTime endDate = new DateTime(2010, 10, 16);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(8, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(18, 00, 0);
            endHour = endHour.Date + ts;

            //
            string msg = "computer enter and exit the lab during the report";
            LabReport lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(52, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");

            Activity act1 = new Activity();
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2010, 10, 10, 10, 0, 0);
            act1.Logout = new DateTime(2010, 10, 10, 19, 0, 0);
            act1.Mode = ActivityType.User;

            //
            msg = "computer enter and exit the lab during the report and have one activity";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(8, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(52, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(((double)8 /52)*100, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(((double)8 / 52) * 100, lr.AverageUsage, msg + "- avarageUsage of lab ");

        }


        [TestMethod]
        public void TestReportsWithOneComputer()
        {

            Lab lab = new Lab();
            lab.LabId = 3000;
            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2010, 2, 15);
            cl.Exit = new DateTime(2010, 3, 15,10,0,0);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);

            Activity act1 = new Activity();
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2010, 2, 16, 10, 0, 0);
            act1.Logout = new DateTime(2010, 2, 16, 12, 0, 0);
            act1.Mode = ActivityType.User;

            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2010, 2, 16);
            DateTime endDate = new DateTime(2010, 2, 17);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;


            //2h out of 12h 
            string msg = "one 2 hour activity";
            LabReport lr = model.CreateLabReport(startDate, endDate, startHour, endHour,lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2,item.GetComputerTotalActiveTime().Hours, msg+": computer total activity time ");
                Assert.AreEqual(10, item.GetComputerTotalTime(), msg+ ": computer total time");

                Assert.AreEqual(20,item.AverageUsage , msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(20,lr.AverageUsage, msg + "- avarageUsage of lab " );

            //
            msg = "one hour of the activity should be included in the report out of 2 hours";
            startHour = new DateTime().Date + new TimeSpan(11,0,0);
            endHour = new DateTime().Date + new TimeSpan(13, 0, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(1, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(50, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(50, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "one hour of the activity should be included in the report out of 2 hours";
            startHour = new DateTime().Date + new TimeSpan(9, 0, 0);
            endHour = new DateTime().Date + new TimeSpan(11, 0, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(1, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(50, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(50, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "one hour of the activity should be included in the report out of 1 hours";
            startHour = new DateTime().Date + new TimeSpan(10, 30, 0);
            endHour = new DateTime().Date + new TimeSpan(11, 30, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(1, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(1, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(100, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(100, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "no computer activity should be included in the report out of 2 hours";
            startHour = new DateTime().Date + new TimeSpan(13, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(15, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");


            // two activities 10-12,13-15
            Activity act2 = new Activity();
            act2.Computer = cl.Computer;
            cl.Computer.Activities.Add(act2);
            act2.Login = new DateTime(2010, 2, 16, 13, 0, 0);
            act2.Logout = new DateTime(2010, 2, 16, 15, 0, 0);
            act2.Mode = ActivityType.User;

            //
            msg = "no computer activity should be included in the report out of 1 hours";
            startHour = new DateTime().Date + new TimeSpan(12, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(13, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(1, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "two activities of total 4 hours should be included in the report out of 10 hours";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(4, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(40, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(40, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "two activities of total 2 hours should be included in the report out of 3 hours";
            startHour = new DateTime().Date + new TimeSpan(11, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(14, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(3, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(((double)2/3)*100, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(((double)2 / 3) * 100, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "no activities should be included in the report out of 20 hours";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            startDate = new DateTime(2010, 2, 17);
            endDate = new DateTime(2010, 2, 19);

            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(20, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");

            //
            msg = "no activities computer exit the lab during the report";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(12, 00, 0);
            startDate = new DateTime(2010, 3, 15);
            endDate = new DateTime(2010, 3, 16);

            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab,true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");

        }

        [TestMethod]
        public void TestReportsWithOneComputer_includeClasses()
        {

            Lab lab = new Lab();
            lab.LabId = 3000;
            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2010, 2, 15);
            cl.Exit = new DateTime(2010, 3, 15, 10, 0, 0);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);

            Activity act1 = new Activity();
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2010, 2, 16, 10, 0, 0);
            act1.Logout = new DateTime(2010, 2, 16, 12, 0, 0);
            act1.Mode = ActivityType.User;

            Activity cact1 = new Activity();
            cact1.Computer = cl.Computer;
            cl.Computer.Activities.Add(cact1);
            cact1.Login = new DateTime(2010, 2, 16, 10, 0, 0);
            cact1.Logout = new DateTime(2010, 2, 16, 12, 0, 0);
            cact1.Mode = ActivityType.Class;


            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2010, 2, 16);
            DateTime endDate = new DateTime(2010, 2, 17);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;


            //
            string msg = "2h user activity, 2h user activity - union 2h";
            LabReport lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total activity time (with classes)");

                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");

            cact1.Login = new DateTime(2010, 2, 16, 9, 0, 0);
            cact1.Logout = new DateTime(2010, 2, 16, 11, 0, 0);
            //
            msg = "2h user activity, 2h user activity- union 3h";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(3, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total activity time (with classes)");

                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(30, item.ScheduleAverageUsage, msg + "- avarageUsage of comp ");

            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(30, lr.ScheduleAverageUsage, msg + "- avarageUsage of lab ");

            cact1.Login = new DateTime(2010, 2, 16, 9, 0, 0);
            cact1.Logout = new DateTime(2010, 2, 16, 14, 0, 0);
            //
            msg = "2h user activity, 5h user activity- union 5h";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(5, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total activity time (with classes)");

                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(50, item.ScheduleAverageUsage, msg + "- avarageUsage of comp ");

            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(50, lr.ScheduleAverageUsage, msg + "- avarageUsage of lab ");

            cact1.Login = new DateTime(2010, 2, 16, 13, 0, 0);
            cact1.Logout = new DateTime(2010, 2, 16, 15, 0, 0);
            //
            msg = "2h user activity, 2h user activity- union 4h";
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(4, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total activity time (with classes)");

                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(20, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(40, item.ScheduleAverageUsage, msg + "- avarageUsage of comp ");

            }
            Assert.AreEqual(20, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(40, lr.ScheduleAverageUsage, msg + "- avarageUsage of lab ");

            //
            Activity cact2 = new Activity();
            cact2.Computer = cl.Computer;
            cl.Computer.Activities.Add(cact2);
            cact2.Login = new DateTime(2010, 2, 16, 10, 30, 0);
            cact2.Logout = new DateTime(2010, 2, 16, 12, 30, 0);
            cact2.Mode = ActivityType.Class;


            msg = "1h out of 2h, 1.5h union with class";
            startHour = new DateTime().Date + new TimeSpan(11, 0, 0);
            endHour = new DateTime().Date + new TimeSpan(13, 0, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(1, item.GetComputerTotalActiveTime().TotalHours, msg + ": computer total activity time ");
                Assert.AreEqual(1.5, item.GetComputerTotalActiveTimeWithClasses().TotalHours, msg + ": computer total class activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(50, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(75, item.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of comp ");

            }
            Assert.AreEqual(50, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(75, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");


            //

            msg = "1h out of 2h, class union-1h";
            startHour = new DateTime().Date + new TimeSpan(9, 0, 0);
            endHour = new DateTime().Date + new TimeSpan(11, 0, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(1, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(1, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total class activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");


                Assert.AreEqual(50, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(50, item.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of comp ");

            }
            Assert.AreEqual(50, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(50, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");


            //
            msg = "no user activity out of 2h, class union- 2h";
            startHour = new DateTime().Date + new TimeSpan(13, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(15, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total class activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
                Assert.AreEqual(100, item.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of comp ");

            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(100, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");


            cact2.Login = new DateTime(2010, 2, 16, 10, 30, 0);
            cact2.Logout = new DateTime(2010, 2, 16, 11, 30, 0);

            Activity cact3 = new Activity();
            cact3.Computer = cl.Computer;
            cl.Computer.Activities.Add(cact3);
            cact3.Login = new DateTime(2010, 2, 16, 11, 30, 0);
            cact3.Logout = new DateTime(2010, 2, 16, 12, 30, 0);
            cact3.Mode = ActivityType.Class;

            // two activities 10-12,13-15
            Activity act2 = new Activity();
            act2.Computer = cl.Computer;
            cl.Computer.Activities.Add(act2);
            act2.Login = new DateTime(2010, 2, 16, 13, 0, 0);
            act2.Logout = new DateTime(2010, 2, 16, 15, 0, 0);
            act2.Mode = ActivityType.User;

            //
            msg = "0h out of 1h, class union- 0.5h";
            startHour = new DateTime().Date + new TimeSpan(12, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(13, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().TotalHours, msg + ": computer total activity time ");
                Assert.AreEqual(0.5, item.GetComputerTotalActiveTimeWithClasses().TotalHours, msg + ": computer total class activity time ");

                Assert.AreEqual(1, item.GetComputerTotalTime(), msg + ": computer total time");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(50, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");

            Activity act3 = new Activity();
            act3.Computer = cl.Computer;
            cl.Computer.Activities.Add(act3);
            act3.Login = new DateTime(2010, 2, 16, 8, 0, 0);
            act3.Logout = new DateTime(2010, 2, 16, 9, 0, 0);
            act3.Mode = ActivityType.User;
            Activity act4 = new Activity();
            act3.Computer = cl.Computer;
            cl.Computer.Activities.Add(act4);
            act4.Login = new DateTime(2010, 2, 16, 9, 30, 0);
            act4.Logout = new DateTime(2010, 2, 16, 10, 0, 0);
            act4.Mode = ActivityType.User;
            Activity cact4 = new Activity();
            act3.Computer = cl.Computer;
            cl.Computer.Activities.Add(cact4);
            cact4.Login = new DateTime(2010, 2, 16, 8, 30, 0);
            cact4.Logout = new DateTime(2010, 2, 16, 9, 30, 0);
            cact4.Mode = ActivityType.Class;
            cact1.Login = new DateTime(2010, 2, 16, 14, 0, 0);
            cact1.Logout = new DateTime(2010, 2, 16,16, 0, 0);
            //
            msg = "5.5h out of 10h, class union-7.5h";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(5.5, item.GetComputerTotalActiveTime().TotalHours, msg + ": computer total activity time ");
                Assert.AreEqual(7.5, item.GetComputerTotalActiveTimeWithClasses().TotalHours, msg + ": computer total class activity time ");

                Assert.AreEqual(10, item.GetComputerTotalTime(), msg + ": computer total time");
            }
            Assert.AreEqual((int)55, (int)lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(75, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");

            //
            msg = "2h out of 3h, class union-2.5h";
            startHour = new DateTime().Date + new TimeSpan(11, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(14, 00, 0);
            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(2, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2.5, item.GetComputerTotalActiveTimeWithClasses().TotalHours, msg + ": computer total class activity time ");

                Assert.AreEqual(3, item.GetComputerTotalTime(), msg + ": computer total time");
            }
            Assert.AreEqual(((double)2 / 3) * 100, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(((double)2.5 / 3) * 100, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");

            //
            msg = "no activities should be included in the report out of 20 hours";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            startDate = new DateTime(2010, 2, 17);
            endDate = new DateTime(2010, 2, 19);

            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(0, item.GetComputerTotalActiveTimeWithClasses().Hours, msg + ": computer total class activity time ");

                Assert.AreEqual(20, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");
            Assert.AreEqual(0, lr.ScheduleAverageUsage, msg + "- ScheduleAverageUsage of lab ");

            //
            msg = "no activities computer exit the lab during the report";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(12, 00, 0);
            startDate = new DateTime(2010, 3, 15);
            endDate = new DateTime(2010, 3, 16);

            lr = model.CreateLabReport(startDate, endDate, startHour, endHour, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            foreach (var item in lr.ComputersReport)
            {
                Assert.AreEqual(0, item.GetComputerTotalActiveTime().Hours, msg + ": computer total activity time ");
                Assert.AreEqual(2, item.GetComputerTotalTime(), msg + ": computer total time");

                Assert.AreEqual(0, item.AverageUsage, msg + "- avarageUsage of comp ");
            }
            Assert.AreEqual(0, lr.AverageUsage, msg + "- avarageUsage of lab ");

        }


        [TestMethod]
        public void TestReportsWithNoActtivities()
        {

            Lab lab = new Lab();
            lab.LabId = 3000;
            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2010, 4, 10, 10, 0, 0);
            cl.Exit = new DateTime(2010, 4, 15, 12, 0, 0);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);

            ReportModel model = new ReportModel(controller);
            DateTime startDate = new DateTime(2010, 4, 5);
            DateTime endDate = new DateTime(2010, 4, 10);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            LabReport lr;
            //report duration befor computer exsist in lab
            string msg = "report duration befor computer exsist in lab";
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);

            Assert.AreEqual(0, lr.ComputersReport.Count, "one computer report expected: " + msg);
            Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);


            //report duration after computer exsist in lab
            msg = "report duration after computer exsist in lab";
            startDate = new DateTime(2010, 4, 16);
            endDate = new DateTime(2010, 4, 20);
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);

            Assert.AreEqual(0, lr.ComputersReport.Count, "one computer report expected: " + msg);
            Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);

            //report duration with one computer - no user activities(only off) 
            Activity act1 = new Activity();
            act1.ComputerId = cl.ComputerId;
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2010, 4, 11, 10, 0, 0);
            act1.Logout = new DateTime(2010, 4, 11, 12, 0, 0);
            act1.Mode = ActivityType.Off;

            msg = "report duration with one computer - no user activities(only off)  ";
            startDate = new DateTime(2010, 4, 11);
            endDate = new DateTime(2010, 4, 12);
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);

            //report duration with computer with no activities in this time
            msg = "report duration with computer with no activities in this time";
            startDate = new DateTime(2010, 4, 13);
            endDate = new DateTime(2010, 4, 14);
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);

            Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
            Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);



            Activity act2 = new Activity();
            act2.ComputerId = cl.ComputerId;
            act2.Computer = cl.Computer;
            cl.Computer.Activities.Add(act2);
            act2.Login = new DateTime(2010, 3, 11, 10, 0, 0);
            act2.Logout = new DateTime(2010, 3, 11, 12, 0, 0);
            act2.Mode = ActivityType.User;
            //report duration with computer not in the lab (but have activity in that time)
            msg = "report duration with computer not in the lab (but have activity in that time)";
            startDate = new DateTime(2010, 3, 11);
            endDate = new DateTime(2010, 3, 12);
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);

            Assert.AreEqual(0, lr.ComputersReport.Count, "no computer report expected: " + msg);
            Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);



        }


        [TestMethod]
        public void TestReportsWithNoComputers()
        {

            Lab lab = new Lab();
            lab.LabId = 3000;


            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2010, 5, 10);
            DateTime endDate = new DateTime(2010, 5, 20);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            LabReport lr;
            //lab with no computers
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);
            Assert.AreEqual(0, lr.ComputersReport.Count, "no computers report expected");
            Assert.AreEqual(0, lr.AverageUsage, "no usage- 0 expected");


            ComputerLab cl = new ComputerLab();
            cl.Computer = new Computer();
            cl.Computer.ComputerId = 3000;
            cl.Lab = lab;
            cl.Entrance = new DateTime(2010, 4, 10, 10, 0, 0);
            cl.Exit = new DateTime(2010, 4, 15, 12, 0, 0);
            lab.ComputerLabs.Add(cl);
            cl.Computer.ComputerLabs.Add(cl);


            // lab with no computers in the report duration
            lr = model.CreateLabReport(startDate, endDate, startHour, endDate, lab, true);
            Assert.AreEqual(0, lr.ComputersReport.Count, "no computers report expected");
            Assert.AreEqual(0, lr.AverageUsage, "no usage- 0 expected");


        }
        [TestCleanup]
        public void ClassCleanup()
        {

        }




    }
}
