using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourSquare.SharpSquare.Entities;

namespace FourSquare.SharpSquare.Core
{
    public class FourSquareDuplicatedResponse<T> : FourSquareResponse where T : FourSquareEntity
    {
        public T response
        {
            get;
            set;
        }
    }
}
