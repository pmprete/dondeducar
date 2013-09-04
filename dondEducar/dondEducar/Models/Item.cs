using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dondEducar.Models
{
    public class Item
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public List<string> Telefonos { get; set; }
        public string Email { get; set; }
        public List<Tag> Tags { get; set; }
    }
}