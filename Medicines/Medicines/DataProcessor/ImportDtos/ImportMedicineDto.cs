using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        [Required]
        [Range(0.01,1000.00)]
        public decimal Price { get; set; }
        [Required]
        [EnumDataType(enumType: typeof(Category))]
        [XmlAttribute("category")]
        public int Category { get; set; }
        [Required]
        public string ProductionDate { get; set; } = null!;
        [Required]
        public string ExpiryDate { get; set; } = null!;
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; } = null!;
    }
}
