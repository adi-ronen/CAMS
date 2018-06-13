using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CAMS.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Controllers.LabsController LabsController = new Controllers.LabsController();
            Models.Lab Lab = LabsController.GetLab(6);
            Models.LabDetailsViewModel LabDetailsViewModel = new Models.LabDetailsViewModel(Lab, LabsController);
        }
    }
}
