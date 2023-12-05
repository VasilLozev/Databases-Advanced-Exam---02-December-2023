using Medicines.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportMedicines
    {
        public string Name { get; set; } = null!;
        [Required]
        public string Price { get; set; }
        [Required]
        public PharmacyDto Pharmacy { get; set; } = null!;
    }
}
