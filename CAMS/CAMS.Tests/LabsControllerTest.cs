using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAMS.Controllers;
using CAMS.Models;


namespace CAMS.Tests
{
    [TestClass]
    public class LabsControllerTest
    {



        //[TestMethod]
        //public void TestLabCreateDelete()
        //{
        //    var controller = new LabsController();
        //    Lab lab = new Lab();
        //    lab.LabId = 1000;
        //    lab.DepartmentId = 0;
        //    lab.Building = "0";
        //    lab.RoomNumber = "0";
        //    controller.Create(lab);
        //    Lab cLab = controller.GetLab(lab.LabId);
        //    Assert.AreEqual(cLab.DepartmentId, lab.DepartmentId);
        //    Assert.AreEqual(cLab.Building, lab.Building);
        //    Assert.AreEqual(cLab.Department, lab.Department);
        //    Assert.AreEqual(cLab.RoomNumber, lab.RoomNumber);
        //    Assert.AreEqual(cLab.TodaysClasses, lab.TodaysClasses);

        //    controller.DeleteConfirmed(lab.LabId);
        //    Lab dLab = controller.GetLab(lab.LabId);
        //    Assert.IsNull(dLab);


        //}
        //[TestMethod]
        //public void TestDelete()
        //{
        //    var controller = new LabsController();
        //    var compC = new ComputersController();

        //    int id = 1000;
        //    if (controller.GetLab(id) != null)
        //    {
        //        controller.DeleteConfirmed(id);
        //        Lab dLab = controller.GetLab(id);
        //        Assert.IsNull(dLab);
        //    }

        //    for (int i = 0; i < 10; i++)
        //    {
        //        int cid = 1000 + i;

        //        if (compC.GetComputer(cid) != null)
        //        {
        //            compC.DeleteConfirmed(cid);
        //            Computer comp = compC.GetComputer(cid);
        //            Assert.IsNull(comp);
        //        }

        //    }


        //}



        //[TestMethod]
        //public void TestLabAddRemoveComputers()
        //{
        //    var controller = new LabsController();
        //    var compC = new ComputersController();

        //    int labid = 1000;
        //    Lab lab = controller.GetLab(labid);
        //    if (lab == null)
        //    {

        //        lab = new Lab();
        //        lab.LabId = 1000;
        //        lab.DepartmentId = 0;
        //        lab.Building = "0";
        //        lab.RoomNumber = "0";
        //        controller.Create(lab);
        //    }
        //    for (int i = 0; i < 10; i++)
        //    {
        //        int compId = 1000 + i;
        //        Computer comp = compC.GetComputer(compId);
        //        if (comp == null)
        //        {
        //            comp = new Computer();
        //            comp.ComputerId = compId;
        //            comp.ComputerName = "test" + comp.ComputerId;
        //            compC.Create(comp);
        //        }
        //        controller.AddComputerToLab(comp.ComputerId, lab.LabId);
        //        lab = controller.GetLab(labid);
        //        comp = compC.GetComputer(compId);
        //        Assert.IsTrue(lab.Computers.Contains(comp),"lab doesnt contains "+compId);
        //        Assert.IsTrue(comp.CurrentLab.Equals(lab.LabId),
        //            "current lab of comp "+compId+" should be "+lab.LabId+" not "+comp.CurrentLab);
        //        //Assert.AreEqual(1,comp.ComputerLabs.Count, "");
        //    }
        //    Assert.AreEqual(10,lab.Computers.Count,"unexpected computers in lab");


        //    for (int i = 0; i < 10; i++)
        //    {
        //        int id = 1000 + i;
        //        controller.RemoveComputerFromLab(id, labid);
        //        Computer comp = controller.GetComputer(id);
        //        Assert.IsNull(comp.CurrentLab);
        //        Assert.IsNull(comp.Lab);
        //    //    Assert.AreEqual(1,comp.ComputerLabs.Count);
        //        lab = controller.GetLab(labid);
        //        Assert.IsFalse(lab.Computers.Contains(comp));
        //     //   Assert.AreEqual(1,lab.ComputerLabs.Count);
        //    }


        //    controller.DeleteConfirmed(lab.LabId);
        //    Lab dLab = controller.GetLab(lab.LabId);
        //    Assert.IsNull(dLab);
        //    for (int i = 0; i < 10; i++)
        //    {
        //        int id = 1000 + i;
        //        Computer comp = controller.GetComputer(id);

        //        Assert.IsNull(comp.CurrentLab);
        //        Assert.IsNull(comp.Lab);
        //       // Assert.Equals(comp.ComputerLabs.Count, 1);
        //        compC.DeleteConfirmed(id);
        //    }

        //}


    }
}
