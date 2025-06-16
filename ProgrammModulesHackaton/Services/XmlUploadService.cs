using System;
using System.IO;
using System.Xml.Linq;
using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;

namespace ProgrammModulesHackaton.Services
{
    public class XmlUploadService
    {
        private readonly ObjectService _objectService;
        private readonly AttributeService _attributeService;
        private readonly DecisionService _decisionService;

        public XmlUploadService(
            ObjectService objectService,
            AttributeService attributeService,
            DecisionService decisionService)
        {
            _objectService = objectService;
            _attributeService = attributeService;
            _decisionService = decisionService;
        }

        /// <summary>
        /// Экспортирует один объект с его атрибутами и решениями в XML-файл.
        /// </summary>
        /// <param name="objectId">ID объекта для экспорта.</param>
        /// <param name="filePath">Путь, по которому будет сохранён XML.</param>
        public void ExportToXml(int objectId, string filePath)
        {
            // Получаем объект
            var obj = _objectService.GetById(objectId);
            if (obj == null)
                throw new InvalidOperationException($"Объект с ID={objectId} не найден.");

            // Формируем элемент ControlObject
            var xo = new XElement("ControlObject",
                new XElement("Name", obj.Name),
                new XElement("Address", obj.Address),
                new XElement("Description", obj.Description)
            );

            // Атрибуты
            var attrs = _attributeService.GetAttributesByObjectId(objectId);
            var xAttrs = new XElement("Attributes");
            foreach (var oa in attrs)
            {
                // Получаем имя шаблона атрибута
                var attrDef = _attributeService.GetAttributeById(oa.AttributeId);
                var name = attrDef?.Name ?? $"Attribute_{oa.AttributeId}";
                xAttrs.Add(new XElement("Attribute",
                    new XAttribute("Name", name)
                ));
            }
            if (xAttrs.HasElements)
                xo.Add(xAttrs);

            // Решения
            var decs = _decisionService.GetDecisionsForObject(objectId);
            var xDecs = new XElement("Decisions");
            foreach (var d in decs)
            {
                xDecs.Add(new XElement("Decision",
                    new XElement("Text", d.Text),
                    new XElement("DueDate", d.DueDate.ToString("yyyy-MM-dd")),
                    new XElement("Status", d.Status),
                    new XElement("Responsible", d.Responsible)
                ));
            }
            if (xDecs.HasElements)
                xo.Add(xDecs);

            // Собираем документ
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Objects", xo)
            );

            // Сохраняем
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            doc.Save(filePath);
        }
    }
}
