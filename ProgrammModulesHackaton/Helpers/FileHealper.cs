using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Helpers
{
    public static class FileHealper
    {
        public static void WriteFileList(List<Models.Attachment> attachments)
        {
            if (attachments.Count == 0)
            {
                Console.WriteLine("Файлы не найдены.");
            }
            else
            {
                Console.WriteLine("\nСписок файлов:");
                foreach (var f in attachments)
                {
                    Console.WriteLine($"""
                            ID: {f.Id}
                            Имя: {f.FileName}
                            Тип: {f.FileType}
                            Загружен: {f.UploadedAt:yyyy-MM-dd HH:mm}
                            ---------------------------
                            """);
                }
            }
        }
    }
}
