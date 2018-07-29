using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class NotificationViewModel
    {

        public User User;
        private NotificationsController _lController;
        public List<Notification> Notifications
        {
            get
            {
                return GetNotifications();
            }

        }

        public NotificationViewModel(User user, NotificationsController controller)
        {
            this.User = user;
            this._lController = controller;


        }
        private List<Notification> GetNotifications()
        {
            List<Notification> notifications = new List<Notification>();
            List<Department> departments = _lController.GetUserViewDepartments(User.UserId);

            foreach (var department in departments)
            {
                foreach (var lab in department.Labs)
                {
                    List<Computer> comps;
                    try
                    {
                        comps = lab.Computers.ToList();
                    }
                    catch
                    {
                        comps = _lController.GetLabComputers(lab.LabId);
                    }
                    foreach (var comp in comps)
                    {
                        //check for disconected notification
                        if (User.DisconnectedPeriod != null)
                        {
                            int days = User.DisconnectedPeriod.Value;
                            DateTime disconectedFrom = DateTime.Now.Date.AddDays(-days);
                            List<Activity> offAct;
                            try
                            {
                                offAct = comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == ActivityType.Off && e.Login <= disconectedFrom).ToList();
                            }
                            catch
                            {
                                offAct = _lController.GetCurrentDisconnectedActivity(comp.ComputerId, disconectedFrom);
                            }
                            if (offAct.Count > 0)
                            {
                                //for how long the computer is disconnected
                                days = (int)(DateTime.Now.Date - offAct[0].Login.Date).TotalDays;
                                Notification ntf = new Notification(comp, lab, department.DepartmentName, Constant.NotificationType.Disconnected, days);
                                notifications.Add(ntf);
                                continue;
                            }
                        }
                        //check for unused notification
                        if (User.NotActivePeriod != null)
                        {
                            Activity userAct;
                            try
                            {
                                userAct = comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == ActivityType.User).ToList().FirstOrDefault();
                            }
                            catch
                            {
                                userAct = _lController.GetUserActivity(comp.ComputerId);
                            }
                            //if there is no user connected rigth now
                            if (userAct == null)
                            {
                                //find the last time a user loged out
                                DateTime? lastLogout;
                                try
                                {
                                    lastLogout = comp.Activities.Where(e => e.Mode == ActivityType.User).Max(e => e.Logout);
                                }
                                catch
                                {
                                    lastLogout = _lController.GetLastLogOutTime(comp.ComputerId);
                                }
                                if (lastLogout == null)
                                {
                                    ComputerLab cl = comp.ComputerLabs.Where(e => e.LabId == lab.LabId && !e.Exit.HasValue).ToList().FirstOrDefault();
                                    if (cl != null)
                                    {
                                        //time from computer enterance to lab
                                        int days = (int)(DateTime.Now.Date - cl.Entrance.Date).TotalDays;
                                        //if the days passed from the computer enterance bigger than the given days- make notification 
                                        if (days >= User.NotActivePeriod.Value)
                                        {
                                            Notification ntf = new Notification(comp, lab, department.DepartmentName, Constant.NotificationType.NotUsed, days);
                                            notifications.Add(ntf);

                                        }
                                    }
                                }
                                else if (lastLogout.HasValue)
                                {
                                    //time from last logout
                                    int days = (int)(DateTime.Now.Date - lastLogout.Value.Date).TotalDays;
                                    //if the days passed from the last user activity bigger than the given days- make notification 
                                    if (days >= User.NotActivePeriod.Value)
                                    {
                                        Notification ntf = new Notification(comp, lab, department.DepartmentName, Constant.NotificationType.NotUsed, days);
                                        notifications.Add(ntf);

                                    }
                                }
                            }
                        }



                    }
                }
            }
            return notifications;

        }
    }

    public class Notification
    {
        public Notification(Computer comp,Lab lab, string departmentName, Constant.NotificationType type,int days)
        {
            DepartmentName = departmentName ;
            Building = lab.Building;
            RoomNumber = lab.RoomNumber;
            ComputerName = comp.ComputerName;
            LabId = lab.LabId;
            NotificationType = type;
            Days = days;
        }

        public Constant.NotificationType NotificationType;
        public int Days;
        public string DepartmentName;
        public string Building;
        public string RoomNumber;
        public string ComputerName;
        public int LabId;
    }
}