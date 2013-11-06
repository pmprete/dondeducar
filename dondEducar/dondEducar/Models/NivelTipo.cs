using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dondEducar.Models
{
    public class NivelTipo
    {
        public Tag TipoDeEstablecimiento { get; set; }
        public Tag NivelEducativo { get; set; }
        
        public override string ToString()
        {
            if (this.NivelEducativo == null) 
                return "";

            if (this.NivelEducativo.Valor == "Otros")
            {
                return this.NivelEducativo.Vista;
            }
            else
            {
                return this.NivelEducativo.Vista + " - " + this.TipoDeEstablecimiento.Vista;
            }
                    
        }
    }
}