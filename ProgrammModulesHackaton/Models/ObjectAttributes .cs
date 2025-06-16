using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Models
{
    public class ObjectAttribute
    {
        public int? Id { get; set; } = null;
        public int ObjectId { get; set; }
        public int AttributeId { get; set; }
    }

    public class AttributesName
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string AttributeName { get; set; }
    }
}



