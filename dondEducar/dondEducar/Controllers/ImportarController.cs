using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
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

            query = Query<Tag>.EQ(e => e.Valor, "Primaria");
            var primario = tags.FindOne(query);

            query = Query<Tag>.EQ(e => e.Valor, "Secundaria");
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

                    escuela.Direccion = csvReader.GetField(0); //Domicilio Edificio
                    //csvReader.GetField(1); Domicilio entre calles
                    escuela.Nombre = csvReader.GetField(2); //Nombre del Establecimiento
                    escuela.Codigo = csvReader.GetField(3); //Nombre abreviado
                    escuela.Telefonos = csvReader.GetField(4); //Telefonos
                    escuela.Email = csvReader.GetField(5); //correo_web
                    //csvReader.GetField(6); Nivel y Tipo
                    //csvReader.GetField(7); Dependencia Funcional
                    escuela.Longitud = Convert.ToDouble(csvReader.GetField(8), CultureInfo.InvariantCulture); //Longitud
                    escuela.Latitud = Convert.ToDouble(csvReader.GetField(9), CultureInfo.InvariantCulture); //Latitud
                    //csvReader.GetField(10); GeoJson

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
            
            var categoriaCuota = new Categoria { Nombre = "Gestion", Vista = "Gestión" };
            var publica = new Tag { Valor = "Publica", Vista = "Publica" };
            tags.Insert(publica);
            categoriaCuota.Tags.Add(publica);
            var privada = new Tag { Valor = "Privada", Vista = "Privada" };
            tags.Insert(privada);
            categoriaCuota.Tags.Add(privada);
            categoria.Insert(categoriaCuota);

            var categoriaNivel = new Categoria { Nombre = "NivelEducativo", Vista = "Nivel Educativo" };
            var inicial = new Tag { Valor = "Inicial", Vista = "Inicial" };
            tags.Insert(inicial);
            categoriaNivel.Tags.Add(inicial);
            var primario = new Tag { Valor = "Primaria", Vista = "Primaria" };
            tags.Insert(primario);
            categoriaNivel.Tags.Add(primario);
            var medio = new Tag { Valor = "Secundaria", Vista = "Secundaria" };
            tags.Insert(medio);
            categoriaNivel.Tags.Add(medio);
            var superior = new Tag { Valor = "Superior", Vista = "Superior" };
            tags.Insert(superior);
            categoriaNivel.Tags.Add(superior);
            var otras = new Tag { Valor = "Otras", Vista = "Otras" };
            tags.Insert(otras);
            categoriaNivel.Tags.Add(otras);
            categoria.Insert(categoriaNivel);

            var categoriaTitulo = new Categoria { Nombre = "Titulo", Vista = "Titulo" };
            var tecnico = new Tag { Valor = "Tecnico", Vista = "Tecnico" };
            tags.Insert(tecnico);
            categoriaTitulo.Tags.Add(tecnico);
            categoria.Insert(categoriaTitulo);

            var categoriaTipo = new Categoria { Nombre = "TipoDeEstablecimiento", Vista = "Tipo De Establecimiento" };
            var especial = new Tag { Valor = "Especial", Vista = "Especial" };
            tags.Insert(especial);
            categoriaTipo.Tags.Add(especial);
            var comun = new Tag { Valor = "Comun", Vista = "Comun" };
            tags.Insert(comun);
            categoriaTipo.Tags.Add(comun);
            var adultos = new Tag { Valor = "Adultos", Vista = "Adultos" };
            tags.Insert(adultos);
            categoriaTipo.Tags.Add(adultos);
            var artisitco = new Tag { Valor = "Artistico", Vista = "Artistico" };
            tags.Insert(artisitco);
            categoriaTipo.Tags.Add(artisitco);
            categoria.Insert(categoriaTipo);
        }


    }
}
