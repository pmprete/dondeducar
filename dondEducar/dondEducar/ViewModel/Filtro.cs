using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dondEducar.Models;

namespace dondEducar.ViewModel
{
    public class Filtro
    {
        public bool Filtrado { get; set; }
        public Tag Titulo { get; set; }
        public Tag Gestion { get; set; }
        public Tag TipoDeEstablecimiento { get; set; }
        public Tag NivelEducativo { get; set; }

        public Filtro()
        {
            Titulo = new Tag();
            Gestion = new Tag();
            TipoDeEstablecimiento = new Tag();
            NivelEducativo = new Tag();
        }
    }
}