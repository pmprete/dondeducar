using System;
using System.Web.Mvc;
using FourSquare.SharpSquare.Core;


namespace dondEducar.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string code)
        {
            var sharpSquare = (SharpSquare)Session["SharpSquare"];
            if (!String.IsNullOrWhiteSpace(code))
            {
                var accessToken = sharpSquare.GetAccessToken((string)Session["RedirectUri"], code);
                Session["AccessToken"] = accessToken;
                sharpSquare.SetAccessToken(accessToken);
            }
            return View("Index");
        }

   
        public ActionResult CerrarSesion()
        {
            var sharpSquare = (SharpSquare)Session["SharpSquare"];
            const string accessToken = "";
            Session["AccessToken"] = accessToken;
            sharpSquare.SetAccessToken(accessToken);
           
            return View("Index");
        }


        public ActionResult Informacion()
        {
            return View("Informacion");
        }

        public ActionResult Donaciones()
        {
            return View("Donaciones");
        }

        public ActionResult Contacto()
        {
            return View("Contacto");
        }

    }
}
