using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CAMS.Models;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace CAMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        
        public AccountController()
        {}

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool BGUresult = BGULogin(model.UserName,model.Password);
            if (BGUresult)
            {
                CreateSessionProperties(model.UserName);
                return RedirectToAction("Index", "Labs");
            }
            else
            {
                ModelState.AddModelError("", "ניסיון כניסה לא חוקי");
                return View(model);
            }
        }

        private void CreateSessionProperties(string userName)
        {
            Dictionary<int, AccessType> Accesses = new Dictionary<int, AccessType>();
            User user;
            int user_id = 0;
            using (var db = new CAMS_DatabaseEntities())
            {
                try
                {
                    user = db.Users.Where(u => u.Email.StartsWith(userName + "@")).First();
                    foreach (UserDepartment dep in user.UserDepartments)
                    {
                        Accesses.Add(dep.DepartmentId, dep.AccessType);
                    }
                    user_id = user.UserId;
                }
                catch {
                    System.Diagnostics.Debug.WriteLine("Cant fined user");
                }
            }

            Session["UserId"] = user_id;
            Session["Accesses"] = Accesses;
            Session["SupperUser"] = IsSupprUser(userName);
        }

        private bool IsSupprUser(string userName)
        {
            string SupperUsers = System.Configuration.ConfigurationManager.AppSettings["SupperUsers"];
            if (SupperUsers != null)
            {
                foreach (string user in SupperUsers.ToString().Split(','))
                {
                    if (user == userName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool BGULogin(string username, string password)
        {
            //create Soap envalope for the request
            StringBuilder xml = new StringBuilder();
            xml.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            xml.Append(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
            xml.Append("<soap:Body>");
            xml.Append(@"<validateUser xmlns=""http://bgu-cmsdeploy.bgu.ac.il/"">");
            xml.Append("<uname>" + username + "</uname>");
            xml.Append("<pwd>" + password + "</pwd>");
            xml.Append("</validateUser>");
            xml.Append("</soap:Body>");
            xml.Append("</soap:Envelope>");
            //BGU Authentication service
            return VlidateUser(xml.ToString(), "http://bgu-cc-msdb.bgu.ac.il/BguAuthWebService/AuthenticationProvider.asmx");
        }

        public bool VlidateUser(string xml, string address)
        {
            try
            {
                HttpWebRequest request = CreateWebRequest(address);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(xml);

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse()) // Error occurs here
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        int start_index = soapResult.IndexOf("<validateUserResult>") + 20; //strat index of result
                        int end_index = soapResult.IndexOf("</validateUserResult>"); // end index of result
                        string ans = soapResult.Substring(start_index, end_index - start_index); //answer is between <validateUserResult>true/false</validateUserResult>
                        return bool.Parse(ans);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Labs", new { Logged = true });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}