﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Models
{
    public class ObjectAttribute
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}



