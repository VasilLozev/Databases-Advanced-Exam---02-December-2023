using Medicines.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static Medicines.Common.ValidationConstants;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmaciesDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [XmlElement("PhoneNumber")]
        [Required]
        [StringLength(14)]
        [RegularExpression(PhoneNumberRegEx)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; } = null!;
        [XmlArray]
        public HashSet<ImportMedicineDto> Medicines { get; set; } = new HashSet<ImportMedicineDto>();
    }
}

//Name – text with length [2, 50] (required)
//⦁	PhoneNumber – text with length 14. (required)
//⦁	All phone numbers must have the following structure: three digits enclosed in parentheses, followed by a space, three more digits, a hyphen, and four final digits: 
//⦁	Example-> (123) 456 - 7890
//⦁	IsNonStop – bool  (required)
//⦁	Medicines - collection of type Medicine