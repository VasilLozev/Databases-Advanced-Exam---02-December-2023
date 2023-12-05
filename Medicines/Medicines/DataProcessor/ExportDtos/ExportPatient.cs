using Medicines.Data.Models;
using Medicines.Data.Models.Enums;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class ExportPatient
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]

        public AgeGroup AgeGroup { get; set; }
        [XmlAttribute]
        public string Gender { get; set; }
        [XmlArray("Medicines")]
        public ExportPatientMedicine[] Medicines { get; set; } 
    }
}
