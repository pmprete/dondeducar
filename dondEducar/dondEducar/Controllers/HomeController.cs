using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
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

        public ActionResult Establecimientos(string valorDelTag)
        {
            
            return View("Establecimientos", (object) valorDelTag);
        }


        public String GetEscuelas(string valorDelTag)
        {
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            var query = Query<Establecimiento>.Where(e=> e.Tags.Any(t=> t.Valor == valorDelTag));
            var establecimientosConTag = establecimientos.Find(query);
            var listaDeEstablecimientos = establecimientosConTag.ToArray();


            var listaSerializada = JsonConvert.SerializeObject(
                listaDeEstablecimientos,
                Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return listaSerializada;

        }

        public ActionResult Importar()
        {

            ViewBag.Message = "Seleccione el archivo csv a importar.";
            return View("Importar");
        }

        [HttpPost]
        public ActionResult Importar(FormCollection formCollection)
        {
            InicializarBaseDeDatos();

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
            query = Query<Establecimiento>.Where(x => x.Tags.Contains(publica));
            establecimientos.Remove(query);

            ParsearCsvEscuelas(file, publica);

        }

        private void ParsearCsvEstablecimientosPrivados(HttpPostedFileBase file)
        {
            var tags = Database.GetCollection<Tag>("Tag");
            var query = Query<Tag>.EQ(e => e.Valor, "Privada");
            
            var privada = tags.FindOne(query);
            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            query = Query<Establecimiento>.Where(x => x.Tags.Contains(privada));
            establecimientos.Remove(query);

            ParsearCsvEscuelas(file, privada);

        }

        private void ParsearCsvEscuelas(HttpPostedFileBase file, Tag couta)
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

            query = Query<Tag>.EQ(e => e.Valor, "Otras");
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

                    escuela.Direccion = csvReader.GetField(0);
                    escuela.Nombre = csvReader.GetField(2);
                    escuela.Codigo = csvReader.GetField(3);
                    escuela.Telefonos = csvReader.GetField(4);
                    escuela.Email = csvReader.GetField(5);
                    escuela.Longitud = Convert.ToDouble(csvReader.GetField(8), CultureInfo.InvariantCulture);
                    escuela.Latitud = Convert.ToDouble(csvReader.GetField(9), CultureInfo.InvariantCulture);

                    //Añado los Tags Correspondientes
                    escuela.Tags.Add(couta);

                    if (csvReader.GetField<string>(7).Trim() == "Educ. Tecnica")
                        escuela.Tags.Add(tecnico);

                    //Itero sobre la lista de nivelesTipos separada por "-"
                    var nivelesTipos = csvReader.GetField(6).Split('-');
                    foreach (var nivelTipo in nivelesTipos)
                    {
                        var nivelTipoTrim = nivelTipo.Trim();
                        if (!String.IsNullOrWhiteSpace(nivelTipoTrim))
                        {
                            if (nivelTipoTrim == "Otras")
                            {
                                escuela.Tags.Add(otras);
                            }
                            else
                            {
                                //Busco el nivel correspondiente
                                var nivel = nivelTipoTrim.Substring(0, 3);
                                switch (nivel)
                                {
                                    case "Ini":
                                        escuela.Tags.Add(inicial);
                                        break;
                                    case "Pri":
                                        escuela.Tags.Add(primario);
                                        break;
                                    case "Med":
                                        escuela.Tags.Add(medio);
                                        break;
                                    case "SNU":
                                        escuela.Tags.Add(superior);
                                        break;
                                    default:
                                        throw new DataException("no hay un nivel valido");
                                }
                                //Busco el Tipo de escuela correspondiente
                                var tipo = nivelTipoTrim.Substring(3, 3);
                                switch (tipo)
                                {
                                    case "Com":
                                        escuela.Tags.Add(comun);
                                        break;
                                    case "Esp":
                                        escuela.Tags.Add(especial);
                                        break;
                                    case "Adu":
                                        escuela.Tags.Add(adultos);
                                        break;
                                    case "Art":
                                        escuela.Tags.Add(artistico);
                                        break;
                                    default:
                                        throw new DataException("no hay un tipo valido");
                                }
                            }
                        }

                    }
                    establecimientos.Insert(escuela);
                }
            }
        }


        private void InicializarBaseDeDatos()
        {
            var categoria = Database.GetCollection<Categoria>("Categoria");
            if (categoria.Count() != 0)
                return;
            categoria.RemoveAll();

            var tags = Database.GetCollection<Tag>("Tag");
            tags.RemoveAll();

            var categoriaCuota = new Categoria { Nombre = "Cuota" };
            categoria.Insert(categoriaCuota);
            var publica = new Tag { Categoria = categoriaCuota, Valor = "Publica" };
            tags.Insert(publica);
            var privada = new Tag { Categoria = categoriaCuota, Valor = "Privada" };
            tags.Insert(privada);

            var categoriaNivel = new Categoria { Nombre = "NivelEducaivo" };
            categoria.Insert(categoriaNivel);
            var inicial = new Tag { Categoria = categoriaNivel, Valor = "Inicial" };
            tags.Insert(inicial);
            var primario = new Tag { Categoria = categoriaNivel, Valor = "Primario" };
            tags.Insert(primario);
            var medio = new Tag { Categoria = categoriaNivel, Valor = "Secundario" };
            tags.Insert(medio);
            var superior = new Tag { Categoria = categoriaNivel, Valor = "Superior" };
            tags.Insert(superior);
            var otras = new Tag { Categoria = categoriaNivel, Valor = "Otras" };
            tags.Insert(otras);

            var categoriaTitulo = new Categoria { Nombre = "Titulo" };
            categoria.Insert(categoriaTitulo);
            var tecnico = new Tag { Categoria = categoriaTitulo, Valor = "Tecnico" };
            tags.Insert(tecnico);

            var categoriaTipo = new Categoria { Nombre = "TipoDeEscuela" };
            categoria.Insert(categoriaTipo);
            var especial = new Tag { Categoria = categoriaTipo, Valor = "Especial" };
            tags.Insert(especial);
            var comun = new Tag { Categoria = categoriaTipo, Valor = "Comun" };
            tags.Insert(comun);
            var adultos = new Tag { Categoria = categoriaTipo, Valor = "Adultos" };
            tags.Insert(adultos);
            var artisitco = new Tag { Categoria = categoriaTipo, Valor = "Artistico" };
            tags.Insert(artisitco);
        }

    }
}
