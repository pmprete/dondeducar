using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FourSquare.SharpSquare.Core;
using WebMatrix.WebData;

namespace dondEducar
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private const string ClientId = "GT3S33AR2TROAVTWMR0A32LRFNQHBD2NSYJADY4VINQHCO13";
        private const string ClientSecret = "35ERC44TORCEQJOJZM1APV1BPUDKJ1NVCWWCXR3T4D1UMUFC";
        private const string RedirectUri = "http://localhost:2673/Home/Index";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            if (HttpContext.Current.Session["SharpSquare"] != null) return;

            var sharpSquare = new SharpSquare(ClientId, ClientSecret);
            var autenticateUrl = sharpSquare.GetAuthenticateUrl(RedirectUri);
            Session["AutenticateUrl"] = autenticateUrl;
            Session["RedirectUri"] = RedirectUri;
            Session["SharpSquare"] = sharpSquare;
        }
    }


}