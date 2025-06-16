using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProgrammModulesHackaton.Helpers
{
    public static class ObjectHelper
    {

        public static void WriteObjectList(ObjectAttributeService attributeService, List<ControlObject> allObjects)
        {
            if (allObjects == null || allObjects.Count == 0)
            {
                Console.WriteLine("Список объектов пуст.");
                return;
            }
            foreach (var obj in allObjects)
            {
                Console.WriteLine($"ID: {obj.Id}");
                Console.WriteLine($"Название: {obj.Name}");
                Console.WriteLine($"Адрес: {obj.Address}");
                Console.WriteLine($"Описание: {obj.Description}");
                Console.WriteLine($"Создан: {obj.CreatedAt:dd-MM-yyyy HH:mm}");

                var attributes = attributeService.GetAttributesByObjectId(obj.Id);
                if (attributes.Count > 0)
                {
                    Console.WriteLine("Атрибуты:");
                    foreach (var attr in attributes)
                    {
                        Console.WriteLine($"   - [{attr.Id}] {attr.AttributeName}");
                    }
                }
                else
                {
                    Console.WriteLine("Атрибуты: отсутствуют");
                }

                Console.WriteLine(new string('-', 40)); // Разделитель
            }
        }
    }
}
