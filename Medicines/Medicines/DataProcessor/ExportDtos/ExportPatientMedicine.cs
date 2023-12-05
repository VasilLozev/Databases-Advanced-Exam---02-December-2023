using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Medicine")]
    public class ExportPatientMedicine
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Price { get; set; }
        [XmlElement]
        public string Producer { get; set; }
        [XmlElement]
        public string BestBefore { get; set; } 
        [XmlAttribute]
        [EnumDataType(enumType: typeof(Category))]
        public string Category { get; set;}
    }
}
