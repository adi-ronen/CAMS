using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAMS.Models;
using CAMS.Controllers;
using System.Collections.Generic;

namespace CAMS.Tests
{
    [TestClass]
    public class OccReportsTest
    {
        ReportsController controller = new ReportsController();

        [TestMethod]
        public void TestOccReportsWithOneComputer()
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

            LabOccupancyReport lr;
            //lab with no computers
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, "no computers -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, "no computers -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, "no computers -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, "no computers -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, "no computers -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, "no computers -no avg hour occupancy expected");

            }
        }

        [TestMethod]
        public void TestOccReportsWithNoActtivities()
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

            LabOccupancyReport lr;
            //report duration befor computer exsist in lab
            string msg = "report duration befor computer exsist in lab";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, "no computers -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, "no computers -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, "no computers -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, "no computers -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, "no computers -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, "no computers -no avg hour occupancy expected");

            }

            //report duration after computer exsist in lab
            msg = "report duration after computer exsist in lab";
            startDate = new DateTime(2010, 4, 16);
            endDate = new DateTime(2010, 4, 20);
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg+" -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg+" -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg+" -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected");

            }
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
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected");

            }
            //report duration with computer with no activities in this time
            msg = "report duration with computer with no activities in this time";
            startDate = new DateTime(2010, 4, 13);
            endDate = new DateTime(2010, 4, 14);
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected");

            }

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
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected");

            }
            foreach (var item in lr.ByHours)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected");
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected");
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected");

            }



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
            cl.Exit = new DateTime(2010, 3, 15, 10, 0, 0);
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
            DateTime endDate = new DateTime(2010, 2, 16);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            LabOccupancyReport lr;
            //2h out of 12h 
            string msg = "one 2 hour activity in 1 day";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 1/1");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/1");
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0*8+1*2)/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }
            foreach (var item in lr.ByHours)
            {
                if (item.Hour >=10 &&item.Hour<12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 1/1 on: " + item.Hour);
                    Assert.AreEqual(1, item.MinOccupancy, msg + " -min by Hour: 1/1 on: " + item.Hour);
                    Assert.AreEqual(1, item.AvgOccupancy, msg + " -avg by Hour: 1/1 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            endDate = new DateTime(2010, 2, 17);

            msg = "one 2 hour activity in 2 day";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 1/1");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/1");
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0*8+1*2)/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if (item.Hour >= 10 && item.Hour < 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 1/1 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 0/1 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: (1/1+0/1)/2 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            //
            msg = "one hour of the activity should be included in the report out of 2 hours (out of 2 days)";
            startHour = new DateTime().Date + new TimeSpan(11, 0, 0);
            endHour = new DateTime().Date + new TimeSpan(13, 0, 0);
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 1/1");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/1");
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + "-avg by day: (0/1+1/1)/2");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if (item.Hour >= 10 && item.Hour < 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 1/1 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 0/1 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: (1/1+0/1)/2 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }



            msg = "no computer activity should be included in the report out of 2 hours (out of 2 days)";
            startHour = new DateTime().Date + new TimeSpan(13, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(15, 00, 0);
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
            }

            foreach (var item in lr.ByHours)
            {

                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
            }


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
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
            }

            foreach (var item in lr.ByHours)
            {

                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
            }

            //10-12,13-15
            msg = "two activities of total 4 hours should be included in the report out of 10 hours";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 1/1");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/1");
                    Assert.AreEqual(0.4, item.AvgOccupancy, msg + "-avg by day: 4/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if ((item.Hour >= 10 && item.Hour < 12) || (item.Hour >= 13 && item.Hour < 15))
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 1/1 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 0/1 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: (1/1+0/1)/2 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            //
            msg = "no activities should be included in the report out of 20 hours";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(18, 00, 0);
            startDate = new DateTime(2010, 2, 17);
            endDate = new DateTime(2010, 2, 19);

            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
            }

            foreach (var item in lr.ByHours)
            {

                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
            }

            //
            msg = "no activities computer exit the lab during the report";
            startHour = new DateTime().Date + new TimeSpan(8, 00, 0);
            endHour = new DateTime().Date + new TimeSpan(12, 00, 0);
            startDate = new DateTime(2010, 3, 15);
            endDate = new DateTime(2010, 3, 16);

            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
            }

            foreach (var item in lr.ByHours)
            {

                Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
            }

        }

        [TestMethod]
        public void TestReportsWithComputers()
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

            ComputerLab cl2 = new ComputerLab();
            cl2.Computer = new Computer();
            cl2.Computer.ComputerId = 3001;
            cl2.Lab = lab;
            cl2.Entrance = new DateTime(2010, 2, 15);
            cl2.Exit = new DateTime(2010, 3, 15, 10, 0, 0);
            lab.ComputerLabs.Add(cl2);
            cl2.Computer.ComputerLabs.Add(cl2);


            Activity act1 = new Activity();
            act1.Computer = cl.Computer;
            cl.Computer.Activities.Add(act1);
            act1.Login = new DateTime(2010, 2, 16, 10, 0, 0);
            act1.Logout = new DateTime(2010, 2, 16, 12, 0, 0);
            act1.Mode = ActivityType.User;

            Activity act2 = new Activity();
            act2.Computer = cl2.Computer;
            cl2.Computer.Activities.Add(act2);
            act2.Login = new DateTime(2010, 2, 16, 10, 0, 0);
            act2.Logout = new DateTime(2010, 2, 16, 12, 0, 0);
            act2.Mode = ActivityType.User;


            ReportModel model = new ReportModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = new DateTime(2010, 2, 16);
            DateTime endDate = new DateTime(2010, 2, 16);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            LabOccupancyReport lr;
            //2h out of 12h (for 2/2 computers)
            string msg = "one 2 hour activity in 1 day of both computers";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 2/2");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/2");
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0*8+1*2)/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }
            foreach (var item in lr.ByHours)
            {
                if (item.Hour >= 10 && item.Hour < 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.MinOccupancy, msg + " -min by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.AvgOccupancy, msg + " -avg by Hour: 2/2 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            endDate = new DateTime(2010, 2, 17);

            msg = "one 2 hour activity in 2 day (for both computers)";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 2/2");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/2");
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0*8+1*2)/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if (item.Hour >= 10 && item.Hour < 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 0/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: (2/2+0/2)/2 on: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            endDate = new DateTime(2010, 2, 16);
            Activity act3 = new Activity();
            act3.Computer = cl2.Computer;
            cl2.Computer.Activities.Add(act3);
            act3.Login = new DateTime(2010, 2, 16, 11, 30, 0);
            act3.Logout = new DateTime(2010, 2, 16, 12, 10, 0);
            act3.Mode = ActivityType.User;

            act2.Login = new DateTime(2010, 2, 16, 11, 10, 0);
            act2.Logout = new DateTime(2010, 2, 16, 11, 20, 0);

            act1.Login = new DateTime(2010, 2, 16, 12, 30, 0);
            act1.Logout = new DateTime(2010, 2, 16, 13, 40, 0);

            msg = "activities 1 day (for 2 computers)";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 2/2");
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/2");
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0.5+1+0.5)/10");
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if (item.Hour ==11)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else if (item.Hour == 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.MinOccupancy, msg + " -min by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.AvgOccupancy, msg + " -avg by Hour: 2/2: " + item.Hour);
                }
                else if (item.Hour == 13)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

            }

            endDate = new DateTime(2010, 2, 17);
            Activity act4 = new Activity();
            act4.Computer = cl.Computer;
            cl.Computer.Activities.Add(act4);
            act4.Login = new DateTime(2010, 2, 17, 12, 10, 0);
            act4.Logout = new DateTime(2010, 2, 17, 12, 40, 0);
            act4.Mode = ActivityType.User;

            Activity act5 = new Activity();
            act5.Computer = cl.Computer;
            cl.Computer.Activities.Add(act5);
            act5.Login = new DateTime(2010, 2, 17, 12, 50, 0);
            act5.Logout = new DateTime(2010, 2, 17, 15, 10, 0);
            act5.Mode = ActivityType.User;

            Activity act6 = new Activity();
            act6.Computer = cl2.Computer;
            cl2.Computer.Activities.Add(act6);
            act6.Login = new DateTime(2010, 2, 17, 12, 05, 0);
            act6.Logout = new DateTime(2010, 2, 17, 12, 10, 0);
            act6.Mode = ActivityType.User;

            msg = "activities 1 day (for 2 computers)";
            lr = model.CreateOccupancyLabReport(startDate, endDate, startHour, endHour, lab, true);
            foreach (var item in lr.ByDay)
            {
                if (item.DayOfWeek == act1.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 2/2 on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/2 on: " + item.DayOfWeek);
                    Assert.AreEqual(0.2, item.AvgOccupancy, msg + "-avg by day: (0.5+1+0.5)/10 on: " + item.DayOfWeek);
                }
                else if (item.DayOfWeek == act4.Login.DayOfWeek)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by day: 2/2 on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by day: 0/2 on: " + item.DayOfWeek);
                    Assert.AreEqual(0.25, item.AvgOccupancy, msg + "-avg by day: (0.5+1+0.5)/10 on: " + item.DayOfWeek);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min day occupancy expected on: " + item.DayOfWeek);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg day occupancy expected on: " + item.DayOfWeek);
                }

            }

            foreach (var item in lr.ByHours)
            {
                if (item.Hour == 11)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.25, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else if (item.Hour == 12)
                {
                    Assert.AreEqual(1, item.MaxOccupancy, msg + " -max by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.MinOccupancy, msg + " -min by Hour: 2/2 on: " + item.Hour);
                    Assert.AreEqual(1, item.AvgOccupancy, msg + " -avg by Hour: 2/2: " + item.Hour);
                }
                else if (item.Hour == 13)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.5, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else if (item.Hour == 14)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.25, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else if (item.Hour == 15)
                {
                    Assert.AreEqual(0.5, item.MaxOccupancy, msg + " -max by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -min by Hour: 1/2 on: " + item.Hour);
                    Assert.AreEqual(0.25, item.AvgOccupancy, msg + " -avg by Hour: 1/2: " + item.Hour);
                }
                else
                {
                    Assert.AreEqual(0, item.MaxOccupancy, msg + " -no max hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.MinOccupancy, msg + " -no min hour occupancy expected on: " + item.Hour);
                    Assert.AreEqual(0, item.AvgOccupancy, msg + " -no avg hour occupancy expected on: " + item.Hour);
                }

                var a = lr.GetDaysAvg();
                var b = lr.GetDaysOfWeek();
                var c = lr.GetHours();
                var d = lr.GetHoursMax();

            }


        }
    }
}
