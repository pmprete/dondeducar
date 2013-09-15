using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dondEducar.Models;

namespace dondEducar.ViewModel
{
    public class EstablecimientoViewModel
    {
        public string NivelEducaivo { get; set; }
        public string JsonEstablecimientos { get; set; }
        public List<Categoria> Categorias {get; set; }

        public EstablecimientoViewModel()
        {
            Categorias = new List<Categoria>();
        }

    }
}