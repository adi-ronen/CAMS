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
            //controller.testAddLab(1000);

            //controller.testAddLab(1001);
            //controller.testAddComp(1000);
            //controller.testAddCompLab(1000, 1001, DateTime.Now.AddDays(-10), null);
            //controller.testAddActivity(1000, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-20).AddHours(3),Constant.ActivityMode.User);
            //controller.testAddActivity(1000, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-5).AddHours(3), Constant.ActivityMode.Off);

            //controller.testAddLab(2000);
            //controller.testAddComp(2000);
            //controller.testAddCompLab(2000, 2000, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-10));
            //controller.testAddActivity(2000, DateTime.Now.AddDays(-18).AddHours(-10), DateTime.Now.AddDays(-18).AddHours(-8), Constant.ActivityMode.User); //2 hours



        }

        [TestMethod]
        public void TestReportsWithOneComputer()
        {
            ReportViewModel model = new ReportViewModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = DateTime.Now.AddDays(-18).AddHours(-15);
            DateTime endDate = DateTime.Now.AddDays(-18).AddHours(2);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;


            list.Add(2000);
            //one 2 hour activity
            string msg = "one 2 hour activity";
            List<LabReport> lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: " + msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
                foreach (var item in lr.ComputersReport)
                {
                    Assert.IsTrue(item.AverageUsage > 19.8 && item.AverageUsage < 20.2,msg+ "- avarageUsage of comp: "+ item.AverageUsage);
                }
                Assert.IsTrue(lr.AverageUsage > 19.8 && lr.AverageUsage < 20.2,msg+ "- avarageUsage of lab: " + lr.AverageUsage);
            }
        }


        [TestMethod]
        public void TestReportsWithNoActtivities()
        {
            ReportViewModel model = new ReportViewModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = DateTime.Now.AddDays(-5);
            DateTime endDate = DateTime.Now;
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            
            list.Add(1001);
            //report duration befor computer exsist in lab
            string msg = "report duration befor computer exsist in lab";
            List<LabReport>  lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: "+msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: "+msg);
                Assert.AreEqual(0, lr.AverageUsage, "no usage expected: "+msg);
            }

            //report duration after computer exsist in lab
            msg = "report duration after computer exsist in lab";
            startDate = DateTime.Now.AddDays(5);
            endDate = DateTime.Now.AddDays(10);
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: " +msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: "+msg);
                Assert.AreEqual(0, lr.AverageUsage, "no usage expected: "+msg);
            }

            //report duration with one computer - no user activities(only off)  
            msg = "report duration with one computer - no user activities(only off)  ";
            startDate = DateTime.Now.AddDays(-10);
            endDate = DateTime.Now;
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: "+msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: "+msg);
                Assert.AreEqual(0, lr.AverageUsage, "no usage expected: "+msg);
            }

            //report duration with computer with no activities in this time
            msg = "report duration with computer with no activities in this time";
            startDate = DateTime.Now.AddDays(-1);
            endDate = DateTime.Now;
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: " + msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(1, lr.ComputersReport.Count, "one computer report expected: " + msg);
                Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);
            }

            //report duration with computer not in the lab (but have activity in that time)
            msg = "report duration with computer not in the lab (but have activity in that time)";
            startDate = DateTime.Now.AddDays(-25);
            endDate = DateTime.Now.AddDays(-15);
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report: " + msg);
            foreach (var lr in lrl)
            {
                Assert.AreEqual(0, lr.ComputersReport.Count, "no computer report expected: " + msg);
                Assert.AreEqual(0, lr.AverageUsage, "no usage expected: " + msg);
            }

        }


        [TestMethod]
        public void TestReportsWithNoComputers()
        {
            ReportViewModel model = new ReportViewModel(controller);
            List<int> list = new List<int>();
            DateTime startDate = DateTime.Now.AddDays(-20);
            DateTime endDate = DateTime.Now.AddDays(-15);
            DateTime startHour = new DateTime();
            TimeSpan ts = new TimeSpan(9, 00, 0);
            startHour = startHour.Date + ts;

            DateTime endHour = new DateTime();
            ts = new TimeSpan(19, 00, 0);
            endHour = endHour.Date + ts;

            //no lab
            List<LabReport> lrl= model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(0, lrl.Count,"no labs to report was inserted");
            
            //lab with no computers
            list.Add(1000);
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count,"should be one report");
            foreach (var lr in lrl)
            {
                Assert.AreEqual(0, lr.ComputersReport.Count, "no computers report expected");
                Assert.AreEqual(0, lr.AverageUsage, "no usage- 0 expected");
            }
            list.Remove(1000);


            // lab with no computers in the report duration
            list.Add(1001);
            lrl = model.CreateLabsReport(list, startDate, endDate, startHour, endDate);
            Assert.AreEqual(1, lrl.Count, "should be one report");
            foreach (var lr in lrl)
            {
                Assert.AreEqual(0, lr.ComputersReport.Count, "no computers report expected");
                Assert.AreEqual(0, lr.AverageUsage, "no usage- 0 expected");
            }


        }
        [TestCleanup]
        public void ClassCleanup()
        {
           // controller.testDeleteLab(1000);

            //controller.testDeleteLab(1001);
            //controller.testDeleteComp(1000);

        }




    }
}
