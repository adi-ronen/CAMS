using System;
using CAMS.Controllers;
using CAMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CAMS.Tests
{
    [TestClass]
    public class NotificationTest
    {
        [TestMethod]
        public void NoNotificationTest()
        {
            User user = new User();
            NotificationViewModel vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(0, vm.Notifications.Count,"user have no notification properties");


            user.DisconnectedPeriod = 2;
            Computer comp = new Computer();
            Lab lab = new Lab();
            comp.Lab = lab;
            lab.Computers.Add(comp);
            Department dep = new Department();
            dep.Labs.Add(lab);
            lab.Department = dep;
            UserDepartment ud = new UserDepartment();
            ud.Department = dep;
            ud.User = user;
            user.UserDepartments.Add(ud);



        }
    }
}
