using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dondEducar.Models
{
    public class Mensaje : MongoEntity
    {
        public Guid UserId { get; set; }
        public string Valor { get; set; }
        public DateTime Fecha { get; set; }
    }
}