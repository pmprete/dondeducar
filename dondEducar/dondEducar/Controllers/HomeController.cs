using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
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

        public ActionResult Establecimientos()
        {
            ViewBag.Message = "Your app description page.";
            //var collection = Database.GetCollection<Establecimiento>("Establecimiento");

            //var entity = new Establecimiento { Nombre = "Tom" };
            //collection.Insert(entity);

            //var id = entity.Id;
            //var query = Query<Establecimiento>.EQ(e => e.Id, id);
            //entity = collection.FindOne(query);

            //entity.Nombre = "Dick";
            //collection.Save(entity);

            //var update = Update<Establecimiento>.Set(e => e.Nombre, "Harry");
            //collection.Update(query, update);

            //collection.Remove(query);


            return View();
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
                            ParsearCsvEscuelasPublicas(file);
                            break;
                        default:
                            ModelState.AddModelError("FileIndefinido",
                                                "Nombre del archivo no valido debe ingresar un archivo .csv valido");
                            break;
                    }

                }
            }

            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            var cursor = establecimientos.FindAll();
            return View("Establecimientos");
        }

        private void ParsearCsvEscuelasPublicas(HttpPostedFileBase file)
        {
            var categoria = Database.GetCollection<Categoria>("Categoria");
            categoria.RemoveAll();

            var tags = Database.GetCollection<Tag>("Tag");
            tags.RemoveAll();
            
            var categoriaCuota = new Categoria {Nombre = "Cuota"};
            categoria.Insert(categoriaCuota);
            var publica = new Tag {Categoria = categoriaCuota, Valor = "Publica"};
            tags.Insert(publica);

            var categoriaNivel = new Categoria {Nombre = "NivelEducaivo"};
            categoria.Insert(categoriaNivel);
            var inicial = new Tag {Categoria = categoriaNivel, Valor = "Inicial"};
            tags.Insert(inicial);
            var primario = new Tag {Categoria = categoriaNivel, Valor = "Primario"};
            tags.Insert(primario);
            var medio = new Tag {Categoria = categoriaNivel, Valor = "Secundario"};
            tags.Insert(medio);
            var superior = new Tag {Categoria = categoriaNivel, Valor = "Superior"};
            tags.Insert(superior);
            var otras = new Tag { Categoria = categoriaNivel, Valor = "Otras" };
            tags.Insert(otras);

            var categoriaTitulo = new Categoria {Nombre = "Titulo"};
            categoria.Insert(categoriaTitulo);
            var tecnico = new Tag {Categoria = categoriaTitulo, Valor = "Tecnico"};
            tags.Insert(tecnico);

            var categoriaTipo = new Categoria {Nombre = "TipoDeEscuela"};
            categoria.Insert(categoriaTipo);
            var especial = new Tag {Categoria = categoriaTipo, Valor = "Especial"};
            tags.Insert(especial);
            var comun = new Tag {Categoria = categoriaTipo, Valor = "Comun"};
            tags.Insert(comun);
            var adultos = new Tag {Categoria = categoriaTipo, Valor = "Adultos"};
            tags.Insert(adultos);
            var artisitco = new Tag {Categoria = categoriaTipo, Valor = "Artistico"};
            tags.Insert(artisitco);


            var establecimientos = Database.GetCollection<Establecimiento>("Establecimiento");
            establecimientos.RemoveAll();
            
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
                    escuela.Tags.Add(publica);

                    if (csvReader.GetField<string>(7).Trim() == "Educ. Tecnica")
                        escuela.Tags.Add(tecnico);

                    //Itero sobre la lista de nivelesTipos separada por "-"
                    var nivelesTipos = csvReader.GetField(6).Split('-');
                    foreach (var nivelTipo in nivelesTipos)
                    {
                        var nivelTipoTrim = nivelTipo.Trim();
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
                                    escuela.Tags.Add(artisitco);
                                    break;
                                default:
                                    throw new DataException("no hay un tipo valido");
                            }
                        }

                    }
                    establecimientos.Insert(escuela);
                }
            }
        }

        public String GetEscuelas(int? tagId)
        {
           
            var collection = Database.GetCollection<Establecimiento>("Establecimiento");
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
