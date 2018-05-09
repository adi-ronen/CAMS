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
            foreach (var department in User.UserDepartments)
            {
                foreach (var lab in department.Department.Labs)
                {
                    foreach (var comp in lab.Computers)
                    {

                        //check for disconected notification
                        if (User.DisconnectedPeriod != null)
                        {
                            int days = User.DisconnectedPeriod.Value;
                            DateTime disconectedFrom = DateTime.Now.Date.AddDays(-days);

                            List<Activity> offAct = comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == (byte)Constant.ActivityMode.Off && e.Login <= disconectedFrom).ToList();
                            if (offAct.Count > 0)
                            {
                                //for how long the computer is disconnected
                                days = (int)(DateTime.Now.Date - offAct[0].Login.Date).TotalDays;
                                Notification ntf = new Notification(comp, Constant.NotificationType.Disconnected, days);
                                notifications.Add(ntf);
                                continue;
                            }
                        }
                        //check for unused notification
                        if (User.NotActivePeriod != null)
                        {
                            List<Activity> userAct = comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == (byte)Constant.ActivityMode.User).ToList();
                            //if there is no user connected rigth now
                            if (userAct.Count == 0)
                            {
                                //find the last time user loged out
                                DateTime? lastLogout = comp.Activities.Where(e => e.Mode == (byte)Constant.ActivityMode.User).Max(e => e.Logout);
                                if (lastLogout == null)
                                {
                                    ComputerLab cl = comp.ComputerLabs.Where(e => e.LabId == lab.LabId && !e.Exit.HasValue).ToList().First();
                                    if (cl != null)
                                    {
                                        //time from computer enterance to lab
                                        int days = (int)(DateTime.Now.Date - cl.Entrance.Date).TotalDays;
                                        //if the days passed from the computer enterance bigger than the given days- make notification 
                                        if (days >= User.NotActivePeriod.Value)
                                        {
                                            Notification ntf = new Notification(comp, Constant.NotificationType.NotUsed, days);
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
                                        Notification ntf = new Notification(comp, Constant.NotificationType.NotUsed, days);
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
        public Notification(Computer comp, Constant.NotificationType type,int days)
        {
            DepartmentName = comp.Lab.Department.DepartmentName ;
            Building = comp.Lab.Building;
            RoomNumber = comp.Lab.RoomNumber;
            ComputerName = comp.ComputerName;
            LabId = comp.Lab.LabId;
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