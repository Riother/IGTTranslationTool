﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGTLocalizer.Model
{
    class ParsedJson
    {
        public int code { get; set; }
        public string lang { get; set; }
        public string[] text { get; set; }
    }
}
