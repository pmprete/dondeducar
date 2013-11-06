using System;
using System.Collections.Generic;
using System.Linq;

namespace dondEducar.Models
{
    public class Tag : MongoEntity
    {
        public string Valor { get; set; }
        public string Vista { get; set; }
        public string CategoriaNombre { get; set; }
    }
}