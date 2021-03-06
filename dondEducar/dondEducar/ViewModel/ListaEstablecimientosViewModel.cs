﻿using System.Collections.Generic;
using dondEducar.Models;

namespace dondEducar.ViewModel
{
    public enum Ordenamiento { MayorPuntaje, MenorPuntaje, NombreAscendente, NombreDescendente };

    public class ListaEstablecimientosViewModel
    {
        public bool EsMapa { get; set; }
        public int Pagina { get; set; }
        public int TotalDePaginas { get; set; }
        public List<Establecimiento> Establecimientos { get; set; }
        public string GeoJsonEstablecimientos { get; set; }
        public List<Categoria> Categorias {get; set; }
        public Filtro Filtro { get; set; }
        public Ordenamiento Orden { get; set; }

        public ListaEstablecimientosViewModel()
        {
            Establecimientos = new List<Establecimiento>();
            Categorias = new List<Categoria>();
            Orden = Ordenamiento.MayorPuntaje;
        }

    }
}