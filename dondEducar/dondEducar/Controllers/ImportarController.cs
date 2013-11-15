using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using FourSquare.SharpSquare.Core;
using FourSquare.SharpSquare.Entities;
using MongoDB.Driver.Builders;
using dondEducar.Models;

namespace dondEducar.Controllers
{
    public class ImportarController : BaseController
    {
        
        public ActionResult Importar()
        {
            ViewBag.Message = "Seleccione el archivo csv a importar.";
            return View("Importar");
        }

        public ActionResult AgregarFourSquareIds()
        {
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            var query = Query<Establecimiento>.Where(x => x.FourSquareVenueId == null);
            var listaEstablecimientos = establecimientos.Find(query);

            foreach (var escuela in listaEstablecimientos)
            {
                var fourSquareVenue = BuscarAgregarEstablecimiento(escuela);

                escuela.FourSquareVenueId = fourSquareVenue.id;
                escuela.Likes = fourSquareVenue.likes.count;
                establecimientos.Save(escuela);
            }
            return View("Importar");
        }
        
        [HttpPost]
        public ActionResult Importar(FormCollection formCollection)
        {
            HttpPostedFileBase file = null;
            try
            {
                file = Request.Files[0];

            }
            catch (Exception)
            {
                ModelState.AddModelError("FileInexistente", "Debe seleccionar un archivo a Importar");
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                ModelState.AddModelError("FileInexistente", "Debe seleccionar un archivo a Importar");
            }
            else
            {
                var partes = file.FileName.Split('.');
                if (partes.Last() != "csv")
                {
                    ModelState.AddModelError("FileExtension",
                                                "Extension del archivo no valida debe ingresar un archivo .csv");
                }
                else
                {
                    switch (partes.First())
                    {
                        case "establecimientos-publicos":
                            ParsearCsvEstablecimientosPublicos(file);
                            break;
                        case "establecimientos-privados":
                            ParsearCsvEstablecimientosPrivados(file);
                            break;
                        default:
                            ModelState.AddModelError("FileIndefinido",
                                                "Nombre del archivo no valido debe ingresar un archivo .csv valido");
                            break;
                    }

                }
            }

            ViewBag.Message = "Importacion Finalizada";

            return View("Importar");
        }


        private void ParsearCsvEstablecimientosPublicos(HttpPostedFileBase file)
        {
            var tags = Database.GetCollection<Tag>("Tag");
            var query = Query<Tag>.EQ(e => e.Valor, "Publica");
            var publica = tags.FindOne(query);

            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            query = Query<Establecimiento>.Where(x => x.Gestion.Equals(publica));
            establecimientos.Remove(query);

            ParsearCsvEscuelas(file, publica);

        }

        private void ParsearCsvEstablecimientosPrivados(HttpPostedFileBase file)
        {
            var tags = Database.GetCollection<Tag>("Tag");
            var query = Query<Tag>.EQ(e => e.Valor, "Privada");

            var privada = tags.FindOne(query);
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            query = Query<Establecimiento>.Where(x => x.Gestion.Equals(privada));
            establecimientos.Remove(query);

            ParsearCsvEscuelas(file, privada);

        }

        private void ParsearCsvEscuelas(HttpPostedFileBase file, Tag gestion)
        {

            var tags = Database.GetCollection<Tag>("Tag");

            var query = Query<Tag>.EQ(e => e.Valor, "Inicial");
            var inicial = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Primario");
            var primario = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Secundario");
            var medio = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Superior");
            var superior = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Otros");
            var otras = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Tecnico");
            var tecnico = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Especial");
            var especial = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Comun");
            var comun = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Adultos");
            var adultos = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Artistico");
            var artistico = tags.FindOne(query);


            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
          
            using (var readFile = new StreamReader(file.InputStream))
            {
                var csvReader = new CsvReader(readFile);

                //Comeinzo a iterar sobre los datos
                while (csvReader.Read())
                {
                    //Creo la nueva escuela
                    var escuela = new Establecimiento();

                    escuela.Direccion = csvReader.GetField(0); //Domicilio Edificio
                    //csvReader.GetField(1); Domicilio entre calles
                    escuela.Nombre = csvReader.GetField(2).Replace("ş",""); //Nombre del Establecimiento
                    escuela.Codigo = csvReader.GetField(3); //Nombre abreviado
                    escuela.Telefonos = csvReader.GetField(4); //Telefonos
                    escuela.Email = csvReader.GetField(5); //correo_web
                    //csvReader.GetField(6); Nivel y Tipo
                    //csvReader.GetField(7); Dependencia Funcional
                    escuela.Longitud = Convert.ToDouble(csvReader.GetField(8), CultureInfo.InvariantCulture); //Longitud
                    escuela.Latitud = Convert.ToDouble(csvReader.GetField(9), CultureInfo.InvariantCulture); //Latitud
                    escuela.GeoJson = csvReader.GetField(10); //GeoJson

                    //Añado los Tags Correspondientes
                    escuela.Gestion = gestion;

                    if (csvReader.GetField<string>(7).Trim() == "Educ. Tecnica")
                        escuela.Titulo = tecnico;

                    //Itero sobre la lista de nivelesTipos separada por "-"
                    var nivelesTipos = csvReader.GetField(6).Split('-');
                    foreach (var nivelTipoString in nivelesTipos)
                    {
                        var nivelTipoTrim = nivelTipoString.Trim();
                        if (String.IsNullOrWhiteSpace(nivelTipoTrim)) continue;

                        var nivelTipo = new NivelTipo();

                        if (nivelTipoTrim == "Otras")
                        {
                            nivelTipo.NivelEducativo = otras;
                        }
                        else
                        {
                            //Busco el nivel correspondiente
                            var nivel = nivelTipoTrim.Substring(0, 3);
                            switch (nivel)
                            {
                                case "Ini":
                                    nivelTipo.NivelEducativo = inicial;
                                    break;
                                case "Pri":
                                    nivelTipo.NivelEducativo = primario;
                                    break;
                                case "Med":
                                    nivelTipo.NivelEducativo = medio;
                                    break;
                                case "SNU":
                                    nivelTipo.NivelEducativo = superior;
                                    break;
                                default:
                                    throw new DataException("no hay un nivel valido");
                            }
                            //Busco el Tipo de escuela correspondiente
                            var tipo = nivelTipoTrim.Substring(3, 3);
                            switch (tipo)
                            {
                                case "Com":
                                    nivelTipo.TipoDeEstablecimiento = comun;
                                    break;
                                case "Esp":
                                    nivelTipo.TipoDeEstablecimiento = especial;
                                    break;
                                case "Adu":
                                    nivelTipo.TipoDeEstablecimiento = adultos;
                                    break;
                                case "Art":
                                    nivelTipo.TipoDeEstablecimiento = artistico;
                                    break;
                                default:
                                    throw new DataException("no hay un tipo valido");
                            }
                        }
                        escuela.NivelTipo.Add(nivelTipo);
                    }
                    
                    establecimientos.Insert(escuela);
                }
            }
        }

       
        public ActionResult LimpiarBase()
        {
            ViewBag.Message = "Seleccione el archivo csv a importar.";

            InicializarBaseDeDatos();

            return View("Importar");
        }


        private void InicializarBaseDeDatos()
        {
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            establecimientos.RemoveAll();

            var categoria = Database.GetCollection<Categoria>("Categoria");
            categoria.RemoveAll();

            var tags = Database.GetCollection<Tag>("Tag");
            tags.RemoveAll();

            const string nombreGestion = "Gestion";
            var categoriaCuota = new Categoria { Nombre = nombreGestion, Vista = "Gestión" };
            var publica = new Tag { Valor = "Publica", Vista = "Publica", CategoriaNombre = nombreGestion };
            tags.Insert(publica);
            categoriaCuota.Tags.Add(publica);
            var privada = new Tag { Valor = "Privada", Vista = "Privada", CategoriaNombre = nombreGestion };
            tags.Insert(privada);
            categoriaCuota.Tags.Add(privada);
            categoria.Insert(categoriaCuota);

            const string nombreNivel = "NivelEducativo";
            var categoriaNivel = new Categoria { Nombre = nombreNivel, Vista = "Nivel Educativo" };
            var inicial = new Tag { Valor = "Inicial", Vista = "Inicial", CategoriaNombre = nombreNivel };
            tags.Insert(inicial);
            categoriaNivel.Tags.Add(inicial);
            var primario = new Tag { Valor = "Primario", Vista = "Primario", CategoriaNombre = nombreNivel };
            tags.Insert(primario);
            categoriaNivel.Tags.Add(primario);
            var medio = new Tag { Valor = "Secundario", Vista = "Secundario", CategoriaNombre = nombreNivel };
            tags.Insert(medio);
            categoriaNivel.Tags.Add(medio);
            var superior = new Tag { Valor = "Superior", Vista = "Superior", CategoriaNombre = nombreNivel };
            tags.Insert(superior);
            categoriaNivel.Tags.Add(superior);
            var otras = new Tag { Valor = "Otros", Vista = "Otros", CategoriaNombre = nombreNivel };
            tags.Insert(otras);
            categoriaNivel.Tags.Add(otras);
            categoria.Insert(categoriaNivel);

            const string nombreTitulo = "Titulo";
            var categoriaTitulo = new Categoria { Nombre = nombreTitulo, Vista = "Titulo" };
            var tecnico = new Tag { Valor = "Tecnico", Vista = "Tecnico", CategoriaNombre = nombreTitulo };
            tags.Insert(tecnico);
            categoriaTitulo.Tags.Add(tecnico);
            categoria.Insert(categoriaTitulo);

            const string nombreTipo = "TipoDeEstablecimiento";
            var categoriaTipo = new Categoria { Nombre = nombreTipo, Vista = "Tipo De Establecimiento" };
            var especial = new Tag { Valor = "Especial", Vista = "Especial", CategoriaNombre = nombreTipo };
            tags.Insert(especial);
            categoriaTipo.Tags.Add(especial);
            var comun = new Tag { Valor = "Comun", Vista = "Comun", CategoriaNombre = nombreTipo };
            tags.Insert(comun);
            categoriaTipo.Tags.Add(comun);
            var adultos = new Tag { Valor = "Adultos", Vista = "Adultos", CategoriaNombre = nombreTipo };
            tags.Insert(adultos);
            categoriaTipo.Tags.Add(adultos);
            var artisitco = new Tag { Valor = "Artistico", Vista = "Artistico", CategoriaNombre = nombreTipo };
            tags.Insert(artisitco);
            categoriaTipo.Tags.Add(artisitco);
            categoria.Insert(categoriaTipo);
        }


    }
}
