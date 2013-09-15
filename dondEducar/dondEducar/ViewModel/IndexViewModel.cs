using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dondEducar.Models;

namespace dondEducar.ViewModel
{
    public class IndexViewModel
    {
        public List<Tag> NivelesEducativos { get; set; }

        public IndexViewModel()
        {
            NivelesEducativos = new List<Tag>();
        }
    }
}