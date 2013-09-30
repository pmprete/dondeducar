using System;
using System.Collections.Generic;
using System.Linq;


namespace dondEducar.Models
{
    public class Establecimiento : MongoEntity
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Direccion { get; set; }
        public string Telefonos { get; set; }
        public string Email { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public List<Tag> TipoDeEstablecimiento { get; set; }
        public Tag Titulo { get; set; }
        public List<Tag> NivelEducativo { get; set; }
        public Tag Gestion { get; set; }

        public Establecimiento()
        {
            TipoDeEstablecimiento = new List<Tag>();
            NivelEducativo = new List<Tag>();
        }
    }
}