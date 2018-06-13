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
using System.Threading;
using System.Net.Mail;
using System.Threading.Tasks;

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
            RegisterCacheEntry();
            CheckSchedual();
          //  CheckComputersActivity();
           // sendReportsToUsers();
        }

        private const string collectionCacheItemKey = "collectionCache";
        private const string scheduleCacheItemKey = "classScheduleCache";
        private const string timeOfCollectingSchedule = "00:00";

        private void RegisterCacheEntry()
        {
            if (null == HttpContext.Current.Cache[collectionCacheItemKey])
            {

                HttpContext.Current.Cache.Add(collectionCacheItemKey, "collection", null,
                    DateTime.MaxValue, TimeSpan.FromMinutes(3),
                    CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(CacheItemRemovedCallback));
            }
            if (null == HttpContext.Current.Cache[scheduleCacheItemKey])
            {

                DateTime onceADay = DateTime.ParseExact(timeOfCollectingSchedule, "H:mm", null, System.Globalization.DateTimeStyles.None);
                onceADay=onceADay.AddDays(1);

                HttpContext.Current.Cache.Add(scheduleCacheItemKey, "schedule", null,
                    onceADay, Cache.NoSlidingExpiration,
                    CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(CacheItemRemovedCallback));
            }
        }
        // TBD- change url
        private const string DummyPageUrl = "http://localhost:21657/TestCacheTimeout/dummy.aspx";
        private const string Address = "partnermatcheryad2@gmail.com";

        public void CacheItemRemovedCallback(string key,
            object value, CacheItemRemovedReason reason)
        {
            Debug.WriteLine("Cache item " + key + " callback: " + DateTime.Now.ToString());
            HitPage(DummyPageUrl);
            switch (key)
            {
                case collectionCacheItemKey:
                    CheckComputersActivity();
                    break;
                case scheduleCacheItemKey:
                    CheckSchedual();
                    SendReportsToUsers();
                    break;
            }
        }

        private void CheckSchedual()
        {
            //new Thread(() =>
            //{
            //    Thread.CurrentThread.IsBackground = true;
                //activitiesModel.GetClassesSchedule();

            //}).Start();
        }

        private void CheckComputersActivity()
        {
            //activitiesModel.GetComputersActivity();
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    Debug.WriteLine("start of collecting user acttivities");
                    Thread.CurrentThread.IsBackground = true;
                    activitiesModel.GetComputersActivity();
                    Debug.WriteLine("end of collecting user acttivities");
                }catch(Exception ex)
                {
                    Debug.WriteLine("CheckComputersActivity error: " + ex.Message);
                    HitPage(DummyPageUrl);
                }

            });
            Task.WaitAll(new Task[] { t});
           
           // t.Start();
            //new Thread(() =>
            //{
            //    Thread.CurrentThread.IsBackground = true;
            //    activitiesModel.GetComputersActivity();

            //}).Start();
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

            MailMessage mail = new MailMessage(Address, user.Email);
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential("partnermatcheryad2@gmail.com", "olla123456"),
                EnableSsl = true
            };

            mail.Subject = "CAMS Computers Report";
            mail.Body = msg;
            mail.IsBodyHtml = true;
            client.Send(mail);

          
        }

        private void HitPage(string url)
        {
            WebClient client = new WebClient();
            client.DownloadData(url);
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // If the dummy page is hit, then it means we want to add another item

            if (HttpContext.Current.Request.Url.ToString() == DummyPageUrl )
            {
                // Add the item in cache and when succesful, do the work.

                RegisterCacheEntry();
                Debug.WriteLine("beginRequest " + DateTime.Now.ToString());
            }
        }


        void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            String cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            if (null == authCookie)
            {//There is no authentication cookie.
                return;
            }

            FormsAuthenticationTicket authTicket = null;

            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception)
            {
                //Write the exception to the Event Log.
                return;
            }

            if (null == authTicket)
            {//Cookie failed to decrypt.
                return;
            }

            //When the ticket was created, the UserData property was assigned a
            //pipe-delimited string of group names.
            String[] groups = authTicket.UserData.Split(new char[] { '|' });

            //Create an Identity.
            GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

            //This principal flows throughout the request.
            GenericPrincipal principal = new GenericPrincipal(id, groups);

            Context.User = principal;

        }
    }
}
