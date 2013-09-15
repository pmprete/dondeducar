using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using dondEducar.Models;
using dondEducar.ViewModel;

namespace dondEducar.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var categorias = Database.GetCollection<Categoria>("Categoria");
            var query = Query<Categoria>.EQ(x => x.Nombre, "NivelEducativo");
            var categoriaNivelEducativo = categorias.FindOne(query);

            var indexViewModel = new IndexViewModel();
            if (categoriaNivelEducativo != null)
                indexViewModel.NivelesEducativos = categoriaNivelEducativo.Tags;
                                     
            return View(indexViewModel);
        }

        public ActionResult Establecimientos(string nivelEducativo)
        {
            ViewBag.Title = nivelEducativo;

            var categorias = Database.GetCollection<Categoria>("Categoria");
            var query = Query<Categoria>.NE(x => x.Nombre, "NivelEducativo");
            var categoriasParaMostrar = categorias.Find(query);
            var listaCategorias = categoriasParaMostrar.ToList();
            
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            query = Query<Establecimiento>.Where(e => e.Tags.Any(t => t.Valor == nivelEducativo));
            var establecimientosConTag = establecimientos.Find(query);
            var listaDeEstablecimientos = establecimientosConTag.ToArray();


            var listaSerializada = JsonConvert.SerializeObject(
                listaDeEstablecimientos,
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });


            var establecimientoViewModel = new EstablecimientoViewModel
                                               {
                                                   NivelEducaivo = nivelEducativo,
                                                   Categorias = listaCategorias,
                                                   JsonEstablecimientos = listaSerializada
                                               };

            return View("Establecimientos", establecimientoViewModel);
        }

    }
}
