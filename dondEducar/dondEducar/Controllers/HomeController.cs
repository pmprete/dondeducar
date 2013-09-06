using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using dondEducar.Models;

namespace dondEducar.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            var collection = Database.GetCollection<Item>("Item");

            var entity = new Item { Nombre = "Tom" };
            collection.Insert(entity);

            var id = entity.Id;
            var query = Query<Item>.EQ(e => e.Id, id);
            entity = collection.FindOne(query);

            entity.Nombre = "Dick";
            collection.Save(entity);

            var update = Update<Item>.Set(e => e.Nombre, "Harry");
            collection.Update(query, update);

            //collection.Remove(query);


            return View();
        }

        public String GetEscuelas(int? tagId)
        {
           
            var collection = Database.GetCollection<Item>("Item");
            var cursor = collection.FindAll();
            var lista = cursor.ToList();
            
            var listaSerializada = JsonConvert.SerializeObject(
                lista,
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return listaSerializada;
            
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
