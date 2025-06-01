using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class XmlUploadService
    {
        private readonly XmlImportService _importService;

        public XmlUploadService(XmlImportService importService)
        {
            _importService = importService;
        }

        public void PromptAndUploadXml()
        {
            Console.WriteLine("Введите путь к XML-файлу для загрузки:");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Путь не указан.");
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            try
            {
                _importService.ImportFromXml(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте XML: {ex.Message}");
            }
        }
    }
}
