using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using dondEducar.Models;
using dondEducar.ViewModel;

namespace dondEducar.Controllers
{
    public class HomeController : BaseController
    {
        private const int _tamañoDePagina = 25;

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Informacion()
        {
            return View("Informacion");
        }

        public ActionResult Contacto()
        {
            return View("Contacto");
        }


        [HttpGet]
        public ActionResult ListaEstablecimientos(string nivelEducativo)
        {
            ViewBag.Title = nivelEducativo;
            const int pagina = 1;
            var filtro = new Filtro {NivelEducativo = {Valor = nivelEducativo}};

            var establecimientoViewModel = CrearViewModel(false, pagina, filtro);

            return View("Filtrado", establecimientoViewModel);
        }

        private EstablecimientoViewModel CrearViewModel(bool esMapa, int pagina, Filtro filtro)
        {
            IMongoQuery query = null;
            Tag tag = null;

            var listaTagsFiltrados = new List<Tag>();
            var tags = Database.GetCollection<Tag>("Tag");

            query = Query<Tag>.EQ(x => x.Valor, filtro.NivelEducativo.Valor);
            tag = tags.FindOne(query);
            listaTagsFiltrados.Add(tag);
            filtro.NivelEducativo = tag;
            filtro.Filtrado = false;

            if (!String.IsNullOrWhiteSpace(filtro.Titulo.Valor))
            {
                query = Query<Tag>.EQ(x => x.Valor, filtro.Titulo.Valor);
                tag = tags.FindOne(query);
                listaTagsFiltrados.Add(tag);
                filtro.Titulo = tag;
                filtro.Filtrado = true;
            }

            if (!String.IsNullOrWhiteSpace(filtro.Gestion.Valor))
            {
                query = Query<Tag>.EQ(x => x.Valor, filtro.Gestion.Valor);
                tag = tags.FindOne(query);
                listaTagsFiltrados.Add(tag);
                filtro.Gestion = tag;
                filtro.Filtrado = true;
            }

            if (!String.IsNullOrWhiteSpace(filtro.TipoDeEstablecimiento.Valor))
            {
                query = Query<Tag>.EQ(x => x.Valor, filtro.TipoDeEstablecimiento.Valor);
                tag = tags.FindOne(query);
                listaTagsFiltrados.Add(tag);
                filtro.TipoDeEstablecimiento = tag;
                filtro.Filtrado = true;
            }

            var categoriasYaSeleccionadas = listaTagsFiltrados.Select(x => x.CategoriaNombre);
            var categorias = Database.GetCollection<Categoria>("Categoria");
            query = Query<Categoria>.NotIn(x => x.Nombre, categoriasYaSeleccionadas);
            if (filtro.NivelEducativo.Valor != "Secundario")
            {
                query = Query.And(query, Query<Categoria>.NE(x => x.Nombre, "Titulo"));
            }
            var categoriasParaMostrar = categorias.Find(query);
            var listaCategorias = categoriasParaMostrar.ToList();

            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            query = Query<Establecimiento>.Where(e => e.NivelTipo.Any(t => t.NivelEducativo.Valor == filtro.NivelEducativo.Valor));

            if (!String.IsNullOrWhiteSpace(filtro.Titulo.Valor))
            {
                query = Query.And(query, Query<Establecimiento>.EQ(x => x.Titulo.Valor, filtro.Titulo.Valor));
            }
            if (!String.IsNullOrWhiteSpace(filtro.Gestion.Valor))
            {
                query = Query.And(query, Query<Establecimiento>.EQ(x => x.Gestion.Valor, filtro.Gestion.Valor));
            }
            if (!String.IsNullOrWhiteSpace(filtro.TipoDeEstablecimiento.Valor))
            {
                query = Query.And(query, Query<Establecimiento>.Where(e => e.NivelTipo.Any(t => t.TipoDeEstablecimiento.Valor == filtro.TipoDeEstablecimiento.Valor)));
            }
            var establecimientosConTag = establecimientos.Find(query);
            
            var cantidadEstablecimientos = establecimientosConTag.Count();
           
            var listaDeEstablecimientos = new List<Establecimiento>();
            var listaSerializada = "";
            
            if (esMapa)
            {
                var geoJson = String.Join(",", establecimientosConTag
                    .Select(x => @"{ ""type"": ""Feature"" ," + Environment.NewLine
                        + @"""geometry"":" + x.GeoJson + "," + Environment.NewLine
                        + @" ""properties"": {" + Environment.NewLine
                        + @" ""id"":""" + x.Id + @"""," + Environment.NewLine
                        + @" ""nombre"":""" + x.Nombre.Replace(@"""","'") + @"""," + Environment.NewLine
                        + @" ""direccion"": """ + x.Direccion.Replace(@"""", "'") + @"""," + Environment.NewLine
                        + @" ""puntaje"": " + x.Puntaje +", " + Environment.NewLine
                        + @" ""show_on_map"": true" + Environment.NewLine 
                        + @"} } "));
                listaSerializada = @"{ ""type"": ""FeatureCollection""," + Environment.NewLine
                                    + @" ""features"": [" + geoJson + "] }";
            }
            else
            {
                listaDeEstablecimientos = establecimientosConTag
                    .Skip(_tamañoDePagina*(pagina - 1))
                    .Take(_tamañoDePagina)
                    .ToList();
            }
            var totalDePaginas = Convert.ToInt32(Math.Ceiling((double)cantidadEstablecimientos / _tamañoDePagina));
            totalDePaginas = (totalDePaginas == 0) ? 1 : totalDePaginas;

            var establecimientoViewModel = new EstablecimientoViewModel
                                               {
                                                   EsMapa = esMapa,
                                                   Pagina = pagina,
                                                   TotalDePaginas = totalDePaginas,
                                                   Filtro = filtro,
                                                   Categorias = listaCategorias,
                                                   Establecimientos = listaDeEstablecimientos,
                                                   GeoJsonEstablecimientos = listaSerializada,
                                               };
            return establecimientoViewModel;
        }


        [HttpPost]
        public ActionResult ListaEstablecimientos(EstablecimientoViewModel establecimientoViewModel)
        {
            var establecimientoNuevoViewModel = CrearViewModel(establecimientoViewModel.EsMapa, establecimientoViewModel.Pagina, establecimientoViewModel.Filtro);

            return View("Filtrado", establecimientoNuevoViewModel);
        }

        [HttpGet]
        public ActionResult Establecimiento(string establecimientoId)
        {
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            var query = Query<Establecimiento>.EQ(e => e.Id, establecimientoId);
            var establecimiento = establecimientos.FindOne(query);

            return View("Establecimiento", establecimiento);
        }

    }
}
