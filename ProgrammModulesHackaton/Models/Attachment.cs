using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public int ControlObjectId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }  // фото, документ и т.п.
        public byte[] Data { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
