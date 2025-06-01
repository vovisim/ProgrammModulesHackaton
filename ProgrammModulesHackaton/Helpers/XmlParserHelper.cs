using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProgrammModulesHackaton.Helpers
{
    internal class XmlParserHelper
    {
        // Метод для парсинга XML строки или файла и возвращения данных в виде списка объектов
        public static List<Dictionary<string, string>> ParseXml(string xmlContent)
        {
            var result = new List<Dictionary<string, string>>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // Предположим, что у нас есть корневой элемент с несколькими "Item" или "Object"
            XmlNodeList nodes = xmlDoc.SelectNodes("//Object"); // или поменяй на нужный тег

            foreach (XmlNode node in nodes)
            {
                var dict = new Dictionary<string, string>();

                foreach (XmlNode child in node.ChildNodes)
                {
                    dict[child.Name] = child.InnerText;
                }

                result.Add(dict);
            }

            return result;
        }

        // Можно добавить методы для парсинга в конкретные модели
        // Например, ParseToControlObject, ParseToDecision и т.п.
    }
}
