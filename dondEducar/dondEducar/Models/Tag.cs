using System;
using System.Collections.Generic;
using System.Linq;

namespace dondEducar.Models
{
    public class Tag : MongoEntity
    {
        public string Categoria { get; set; }
        public string Valor { get; set; }
    }
}