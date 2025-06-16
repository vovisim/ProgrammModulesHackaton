using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Helpers
{
    public static class DecisionHelper
    {
        public static void WriteDecisionList(List<Decision> decisions)
        {
            foreach (var d in decisions)
            {
                Console.WriteLine($"""
                        ID: {d.Id}
                        Объект ID: {d.ControlObjectId}
                        Текст: {d.Text}
                        Дата исполнения: {d.DueDate:dd.MM.yyyy}
                        Статус: {d.Status}
                        Ответственный: {d.Responsible}
                        -------------------------------
                        """);
            }
        }
    }
}
