﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static Medicines.Common.ValidationConstants;

namespace Medicines.Data.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [XmlElement]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(14)]
        [RegularExpression(PhoneNumberRegEx)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [XmlElement]
        public bool IsNonStop { get; set; } 
        [XmlArray("Medicines")]
        public HashSet<Medicine> Medicines { get; set; } = new HashSet<Medicine>();
    }
}
//⦁	Id – integer, Primary Key
//⦁	Name – text with length [2, 50] (required)
//⦁	PhoneNumber – text with length 14. (required)
//⦁	All phone numbers must have the following structure: three digits enclosed in parentheses, followed by a space, three more digits, a hyphen, and four final digits: 
//⦁	Example-> (123) 456 - 7890
//⦁	IsNonStop – bool  (required)
//⦁	Medicines - collection of type Medicine