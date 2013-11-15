using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FourSquare.SharpSquare.Entities;
using dondEducar.Models;

namespace dondEducar.ViewModel
{
    public class EstablecimientoViewModel
    {
        public Establecimiento Establecimiento { get; set; }
        public FourSquareViewModel FourSquareViewModel { get; set; }
        public string OauthToken { get; set; }
    }
}