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
        public List<Notification> Notifications;

        

        public NotificationViewModel(User user, NotificationsController controller)
        {
            this.User = user;
            this._lController = controller;
            Notifications = new List<Notification>();
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

                            Activity offAct=comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == (byte)Constant.ActivityMode.Off && e.Login<disconectedFrom).First();
                            if (offAct != null)
                            {
                                //for how long the computer is disconnected
                                days = (int)(DateTime.Now.Date - offAct.Login.Date).TotalDays;
                                Notification ntf = new Notification(comp,Constant.NotificationType.Disconnected,days);
                                Notifications.Add(ntf);
                                continue;
                            }
                        }
                        //check for unused notification
                        if (User.NotActivePeriod != null)
                        {
                            Activity userAct = comp.Activities.Where(e => !e.Logout.HasValue && e.Mode == (byte)Constant.ActivityMode.User).First();
                            //if there is no user connected rigth now
                            if (userAct == null)
                            {
                                //find the last time user loged out
                                DateTime? lastLogout = comp.Activities.Where(e => e.Mode == (byte)Constant.ActivityMode.User).Max(e => e.Logout);
                                if (!lastLogout.HasValue)
                                {
                                    //time from last logout
                                    int days = (int)(DateTime.Now.Date - lastLogout.Value.Date).TotalDays;
                                    //if the days passed from the last user activity bigger than the given days- make notification 
                                    if (days >= User.DisconnectedPeriod.Value)
                                    {
                                        Notification ntf = new Notification(comp, Constant.NotificationType.NotUsed, days);
                                        Notifications.Add(ntf);

                                    }
                                }
                            }
                        }



                    }
                }
            }

        }
    }

    public class Notification
    {
        public Notification(Computer comp, Constant.NotificationType type,int days)
        {
            Computer = comp;
            NotificationType = type;
            Days = days;
        }

        public Computer Computer;
        public Constant.NotificationType NotificationType;
        public int Days;
    }
}