using System;
using System.Collections.Generic;
using System.Linq;


namespace dondEducar.Models
{
    public class Establecimiento : MongoEntity
    {
        //Provenientes del CSV
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefonos { get; set; }
        public string Email { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public List<NivelTipo> NivelTipo { get; set; }
        public Tag Titulo { get; set; }
        public Tag Gestion { get; set; }
        public string GeoJson { get; set; }

        //Agregados
        public long Likes { get; set; }
        public string FourSquareVenueId { get; set; }

        public Establecimiento()
        {
            NivelTipo = new List<NivelTipo>();
        }
    }
}