using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class HomeController : Controller, IRequiresSessionState
    {
        private HttpSessionState Session { get { return System.Web.HttpContext.Current.Session; } }

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(EsendexCredentials credentials)
        {
            Session["credentials"] = credentials;

            return Redirect("/Mailbox");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}