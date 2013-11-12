using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using FourSquare.SharpSquare.Core;
using FourSquare.SharpSquare.Entities;
using MongoDB.Driver;
using dondEducar.Models;

namespace dondEducar.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            var url = new MongoUrl(ConnectionString());
            var client = new MongoClient(url);
            var server = client.GetServer();
            _mongoDatabase = server.GetDatabase(url.DatabaseName);
        }

        private readonly MongoDatabase _mongoDatabase;
        public MongoDatabase Database
        {
            get { return _mongoDatabase; }
        }

        private static string ConnectionString()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://localhost/test";
            return connectionString;
        }

        protected Venue BuscarAgregarEstablecimiento(Establecimiento establecimiento)
        {
            var listaNiveles = new List<string>
                                           {
                                               "Inicial",
                                               "Primario",
                                               "Secundario"
                                           };

            Venue fourSquareVenue = null;
            var sharpSquare = (SharpSquare)Session["SharpSquare"];

            if (establecimiento.FourSquareVenueId == null)
            {
                var latitudLongitud = establecimiento.Latitud.ToString(CultureInfo.InvariantCulture.NumberFormat)
                                      + "," +
                                      establecimiento.Longitud.ToString(CultureInfo.InvariantCulture.NumberFormat);

                var parametros = new Dictionary<string, string>();
                parametros.Add("ll", latitudLongitud);
                parametros.Add("intent", "match");
                var nombreReducido = establecimiento.Nombre.ToLower()
                    .Replace("inst.", "")
                    .Replace("instituto", "")
                    .Replace("colegio", "")
                    .Replace("coleg.", "")
                    .Replace("priv.", "")
                    .Replace("integral", "")
                    .Replace("escuela", "")
                    .Replace("educ.", "")
                    .Replace("fund.", "")
                    .Replace("fundacion", "")
                    .Replace("secundaria", "")
                    .Replace("primaria", "")
                    .Replace("infantil", "")
                    .Replace("jardin", "")
                    .Replace("jardin de infantes", "")
                    .Replace("maternal", "")
                    .Replace("parroquial", "")
                    .Trim();
                parametros.Add("query", nombreReducido);

                var categorias = new List<string>();

                //categorias.Add("4bf58dd8d48988d131941735"); //Centro Espiritual
                if (establecimiento.NivelTipo.Any(x => listaNiveles.Contains(x.NivelEducativo.Valor)))
                {
                    categorias.Add("4bf58dd8d48988d13b941735"); //Colegio
                }
                else
                {
                    if (establecimiento.NivelTipo.Any(x => x.NivelEducativo.Valor == "Otros"))
                    {
                        categorias.Add("4bf58dd8d48988d1f2931735"); //Lugar de Artes interpretativas
                    }
                    else
                    {
                        categorias.Add("4d4b7105d754a06372d81259"); //Facultad y Universidad
                    }
                }
                parametros.Add("categoryId", String.Join(",", categorias));

                var listaDeLugares = sharpSquare.SearchVenues(parametros);

                Venue lugar = null;
                //Busco el Id de FourSquare

                lugar = listaDeLugares.OrderBy(x => Math.Abs(x.location.lat - establecimiento.Latitud)
                    + Math.Abs(x.location.lng - establecimiento.Longitud)
                    ).FirstOrDefault();
                if (lugar == null)
                {
                    var venue = new Dictionary<string, string>();
                    venue.Add("name", establecimiento.Nombre);
                    venue.Add("address", establecimiento.Direccion);
                    venue.Add("city", "Buenos Aires");
                    venue.Add("state", "Buenos Aires");
                    //venue.Add("phone", establecimiento.Telefonos); //Tira error por ser una string y no un numero
                    venue.Add("ll", establecimiento.Latitud.ToString(CultureInfo.InvariantCulture.NumberFormat)
                                        + "," +
                                        establecimiento.Longitud.ToString(CultureInfo.InvariantCulture.NumberFormat));

                    if (establecimiento.NivelTipo.Any(x => listaNiveles.Contains(x.NivelEducativo.Valor)))
                    {
                        venue.Add("primaryCategoryId", "4bf58dd8d48988d13b941735");
                    }
                    else
                    {
                        if (establecimiento.NivelTipo.Any(x => x.NivelEducativo.Valor == "Otros"))
                        {
                            venue.Add("primaryCategoryId", "4bf58dd8d48988d1f2931735"); ;
                        }
                        else
                        {
                            venue.Add("primaryCategoryId", "4d4b7105d754a06372d81259");
                        }
                    }

                    fourSquareVenue = sharpSquare.AddVenue(venue);
                    return fourSquareVenue;
                }

                fourSquareVenue = sharpSquare.GetVenue(lugar.id);
                return fourSquareVenue;
            }

            fourSquareVenue = sharpSquare.GetVenue(establecimiento.FourSquareVenueId);
            return fourSquareVenue;
        }
    }
}
