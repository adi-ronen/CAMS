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
            Activity act1 = new Activity();
            act1.Mode = (byte)Constant.ActivityMode.Off;
            act1.Login = DateTime.Now.Date.AddDays(-1);
            comp.Activities.Add(act1);
            act1.Computer = comp;

            vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(0, vm.Notifications.Count, "computer disconnected only 1 day");

            act1.Login = DateTime.Now.Date.AddDays(-3);
            act1.Logout = DateTime.Now.Date;
            vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(0, vm.Notifications.Count, "computer is connected");

            user.NotActivePeriod = 2;
            Activity act2 = new Activity();
            act2.Mode = (byte)Constant.ActivityMode.User;
            act2.Login = DateTime.Now.Date.AddDays(-1);
            comp.Activities.Add(act2);
            act2.Computer = comp;
             vm = new NotificationViewModel(user, new NotificationsController());

            Assert.AreEqual(0, vm.Notifications.Count, "last user activity day ago");




        }

        [TestMethod]
        public void DisconnectedNotificationTest()
        {
            User user = new User();
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
            Activity act1 = new Activity();
            act1.Mode = (byte)Constant.ActivityMode.Off;
            act1.Login = DateTime.Now.Date.AddDays(-3);
            comp.Activities.Add(act1);
            act1.Computer = comp;
            NotificationViewModel vm = new NotificationViewModel(user, new NotificationsController());

            Assert.AreEqual(1, vm.Notifications.Count, "computer disconnected 3 days");
            Assert.AreEqual(Constant.NotificationType.Disconnected, vm.Notifications[0].NotificationType, "notification type should be discconnected");
            Assert.AreEqual(3,vm.Notifications[0].Days , "wrong nomber of disconnected days");

            act1.Login = DateTime.Now.Date.AddDays(-3);
            act1.Logout = DateTime.Now.Date;
            vm = new NotificationViewModel(user, new NotificationsController());

            Assert.AreEqual(0, vm.Notifications.Count, "computer is connected");
           


        }
        [TestMethod]
        public void UnusedNotificationTest()
        {
            User user = new User();
            user.DisconnectedPeriod = 2;
            user.NotActivePeriod = 5;
            Computer comp = new Computer();
            Lab lab = new Lab();
            comp.Lab = lab;
            lab.Computers.Add(comp);
            ComputerLab cl = new ComputerLab();
            cl.Computer = comp;
            cl.Lab = lab;
            cl.Entrance = DateTime.Now.Date.AddDays(-10);
            lab.ComputerLabs.Add(cl);
            comp.ComputerLabs.Add(cl);
            Department dep = new Department();
            dep.Labs.Add(lab);
            lab.Department = dep;
            UserDepartment ud = new UserDepartment();
            ud.Department = dep;
            ud.User = user;
            user.UserDepartments.Add(ud);
            Activity act1 = new Activity();
            act1.Mode = (byte)Constant.ActivityMode.Off;
            act1.Login = DateTime.Now.Date.AddDays(-3);
            comp.Activities.Add(act1);
            act1.Computer = comp;
            NotificationViewModel vm = new NotificationViewModel(user, new NotificationsController());

            Assert.AreEqual(1, vm.Notifications.Count, "computer disconnected 3 days");
            Assert.AreEqual(Constant.NotificationType.Disconnected, vm.Notifications[0].NotificationType, "notification type should be discconnected");
            Assert.AreEqual(3, vm.Notifications[0].Days, "wrong nomber of disconnected days");

            act1.Login = DateTime.Now.Date.AddDays(-3);
            act1.Logout = DateTime.Now.Date;
            vm = new NotificationViewModel(user, new NotificationsController());

            Assert.AreEqual(1, vm.Notifications.Count, "computer is connected but no user activities");
            Assert.AreEqual(Constant.NotificationType.NotUsed, vm.Notifications[0].NotificationType, "notification type should be unused");
            //there are no user activites at all so the days without user activity should be since the computer is in the lab
            Assert.AreEqual(10, vm.Notifications[0].Days, "wrong nomber of disconnected days");


            Activity act2 = new Activity();
            act2.Mode = (byte)Constant.ActivityMode.User;
            act2.Login = DateTime.Now.Date.AddDays(-3);
            comp.Activities.Add(act2);
            act2.Computer = comp;
            vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(0, vm.Notifications.Count, "user loged on");

            act2.Logout = DateTime.Now.Date.AddDays(-1);
            vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(0, vm.Notifications.Count, "no user activity for only 1 day");

            act2.Login = DateTime.Now.Date.AddDays(-6);
            act2.Logout = DateTime.Now.Date.AddDays(-5);
            vm = new NotificationViewModel(user, new NotificationsController());
            Assert.AreEqual(1, vm.Notifications.Count, "no user activity for 3 days");
            Assert.AreEqual(Constant.NotificationType.NotUsed, vm.Notifications[0].NotificationType, "notification type should be unused");
            Assert.AreEqual(5, vm.Notifications[0].Days, "wrong nomber of disconnected days");






        }


    }
}
