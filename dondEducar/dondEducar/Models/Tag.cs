using System;
using System.Collections.Generic;
using System.Linq;

namespace dondEducar.Models
{
    public class Tag : MongoEntity
    {
        public Categoria Categoria { get; set; }
        public string Valor { get; set; }
    }
}