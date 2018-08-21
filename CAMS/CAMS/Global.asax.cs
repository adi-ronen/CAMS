using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Net;
using System.Web.Caching;
using System.Diagnostics;
using CAMS.Controllers;
using CAMS.Models;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace CAMS
{
    public class MvcApplication : System.Web.HttpApplication
    {

        ActivitiesModel activitiesModel;


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            activitiesModel = new ActivitiesModel(new ActivitiesController());
            //Activity_Timer();
            //Schedule_Timer();
            //sendReportsToUsers();
           

        }
        private const string collectionCacheItemKey = "collectionCache";
        private const string scheduleCacheItemKey = "classScheduleCache";
        private const string timeOfCollectingSchedule = "00:00";
        static Timer act_timer,sch_timer;

        private void Activity_Timer()
        {
            double tickTime = (double)(new TimeSpan(0, 3, 00)).TotalMilliseconds;
            act_timer = new Timer(tickTime);
            act_timer.Elapsed += new ElapsedEventHandler(ActivityTimer_Elapsed);
            act_timer.Start();

        }

        private void ActivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("### activity Timer Stopped ### \n");
            act_timer.Stop();
            Console.WriteLine("### activity Task Started ### \n\n");
            CheckComputersActivity();
            Console.WriteLine("### activity Task Finished ### \n\n");
            Activity_Timer();
        }
        private void Schedule_Timer()
        {
            Console.WriteLine("### schedule Timer Started ###");

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 4, 0, 0, 0); 
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            sch_timer = new Timer(tickTime);
            sch_timer.Elapsed += new ElapsedEventHandler(ScheduleTimer_Elapsed);
            sch_timer.Start();

        }

        private void ScheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("### Schedule Timer Stopped ### \n");
            sch_timer.Stop();
            Console.WriteLine("### Scheduled Task Started ### \n\n");
            CheckComputersActivity();
            Console.WriteLine("### Scheduled Task Finished ### \n\n");
            Schedule_Timer();
        }
        
        private void CheckSchedual()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    Debug.WriteLine("start of updating lab schedual");
                    System.Threading.Thread.CurrentThread.IsBackground = true;
                    activitiesModel.GetClassesSchedule();
                    Debug.WriteLine("end of updating lab schedual");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetClassesSchedule error: " + ex.Message);
                }

            });
            Task.WaitAll(new Task[] { t });
        }

        private void CheckComputersActivity()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    Debug.WriteLine("start of collecting user acttivities");
                    System.Threading.Thread.CurrentThread.IsBackground = true;
                    activitiesModel.GetComputersActivity();
                    Debug.WriteLine("end of collecting user acttivities");
                }catch(Exception ex)
                {
                    Debug.WriteLine("CheckComputersActivity error: " + ex.Message);
                }

            });
            Task.WaitAll(new Task[] { t});
           
        }

        private void SendReportsToUsers()
        {
            DateTime today = DateTime.Now.Date;
            // send mail to daily users
            FindUsersAndNotifications(NotificationFrequency.Daily);
            
            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                //send mail to weekly users
                FindUsersAndNotifications(NotificationFrequency.Weekly);

            }
            if (today.Day == 1)
            {
                //send mail to montly
                FindUsersAndNotifications(NotificationFrequency.Monthly);

            }

        }

        private void FindUsersAndNotifications(NotificationFrequency frequency)
        {
            NotificationsController controller = new NotificationsController();

            foreach (var user in controller.GetEmailSubscribers(frequency))
            {
                NotificationViewModel notificationModel = new NotificationViewModel(user, controller);
                List<Notification> notifications = notificationModel.Notifications;
                if (notifications.Count > 0)
                {
                    string msg = "<table>";
                    msg += "<tr> <td> מחלקה </td> <td> בניין </td> <td> כיתה </td> <td> עמדה </td>  <td> פירוט התראה </td><th></th></tr> ";

                    foreach (var notification in notificationModel.Notifications)
                    {
                        msg += "<tr> <td>" + notification.DepartmentName + "</td> ";
                        msg += "<td>" + notification.Building + "</td> ";
                        msg += "<td>" + notification.RoomNumber + "</td> ";
                        msg += "<td>" + notification.ComputerName + "</td> ";
                        switch (notification.NotificationType)
                        {
                            case Constant.NotificationType.Disconnected:
                                msg += "<td> לא מחובר " + notification.Days + " ימים </td> ";
                                break;
                            case Constant.NotificationType.NotUsed:
                                msg += "<td> לא בשימוש " + notification.Days + " ימים </td> ";
                                break;
                        }
                        msg += "</tr>";

                    }
                    msg += "</table>";
                    SendEmail(msg, user);
                }


            }
        }

        private void SendEmail(string msg, User user)
        {

            MailMessage mail = new MailMessage("noreply@bgu.ac.il", user.Email, "CAMS Computers Report", msg);
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.bgu.ac.il",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
            mail.IsBodyHtml = true;
            client.Send(mail);
        }
        
    }
}
