using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourSquare.SharpSquare.Entities
{
    public class DuplicatedVenue : FourSquareEntity
    {
        public string ignoreDuplicatesKey
        {
            get; 
            set;
        }

        public List<Venue> candidateDuplicateVenues
        {
            get;
            set;
        }
    }
}
