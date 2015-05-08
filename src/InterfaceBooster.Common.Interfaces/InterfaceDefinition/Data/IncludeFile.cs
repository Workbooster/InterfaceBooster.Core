﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    public class IncludeFile
    {
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        [XmlAttribute("relativePath")]
        public string RelativePath { get; set; }
    }
}
