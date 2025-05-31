using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Models
{
    public class ObjectAttribute
    {
        public int Id { get; set; }              // PK
        public int ControlObjectId { get; set; } // FK к ControlObject.Id
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }
}
