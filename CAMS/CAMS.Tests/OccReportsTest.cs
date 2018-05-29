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

    }
}
