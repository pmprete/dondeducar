﻿using System;
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
        public List<Tag> Tags { get; set; }

        public Establecimiento()
        {
            Tags = new List<Tag>();
        }
    }
}