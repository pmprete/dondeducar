using System.Collections.Generic;

namespace dondEducar.Models
{
    public class Categoria : MongoEntity
    {
        public string Nombre { get; set; }
        public string Vista { get; set; }
        public List<Tag> Tags { get; set; }

        public Categoria()
        {
            Tags = new List<Tag>();
        }
    }
}