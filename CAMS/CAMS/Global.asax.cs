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
            checkSchedual();
            checkComputersActivity();
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
        private const string DummyPageUrl = "http://localhost:63976/TestCacheTimeout/dummy.aspx";

        public void CacheItemRemovedCallback(string key,
            object value, CacheItemRemovedReason reason)
        {
            Debug.WriteLine("Cache item " + key + " callback: " + DateTime.Now.ToString());
            HitPage(DummyPageUrl);
            switch (key)
            {
                case collectionCacheItemKey:
                    checkComputersActivity();
                    break;
                case scheduleCacheItemKey:
                    checkSchedual();
                    break;
            }
        }

        private void checkSchedual()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                activitiesModel.GetClassesSchedule();

            }).Start();
        }

        private void checkComputersActivity()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                activitiesModel.GetComputersActivity();

            }).Start();
        }

        private void HitPage(string url)
        {
            WebClient client = new WebClient();
            client.DownloadData(url);
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // If the dummy page is hit, then it means we want to add another item

            // in cache
            Debug.WriteLine("beginRequest " + DateTime.Now.ToString());

            if (HttpContext.Current.Request.Url.ToString() == DummyPageUrl )
            {
                // Add the item in cache and when succesful, do the work.

                RegisterCacheEntry();
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
            catch (Exception ex)
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
