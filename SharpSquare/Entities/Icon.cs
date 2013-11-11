﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourSquare.SharpSquare.Entities
{
    public class Icon
    {
        /// <summary>
        /// url of the image withouth the suffix
        /// </summary>
        public string prefix { get; set; }

        /// <summary>
        /// Suffix of the image, example .png .ico etc
        /// </summary>
        public string sufix { get; set; }

        public string primary { get; set; }
    }
}
