using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Models
{
    public class Decision
    {
        public int Id { get; set; }               // PK
        public int ControlObjectId { get; set; }  // FK к ControlObject.Id
        public string Text { get; set; }          // Текст поручения
        public DateTime DueDate { get; set; }     // Контрольная дата исполнения
        public string Status { get; set; }        // Статус (например: "Ожидает", "В работе", "Выполнено", "Просрочено")
        public string Responsible { get; set; }   // Ответственный
    }
}
