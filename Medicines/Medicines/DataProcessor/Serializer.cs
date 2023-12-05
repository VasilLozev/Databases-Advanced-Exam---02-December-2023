namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var patients = context.Patients
                .Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > dateTime)).Distinct()
                .OrderByDescending(p => p.PatientsMedicines.Where(pm=>pm.Medicine.ProductionDate > dateTime).Count())
                .ThenBy(p => p.FullName)
                .Select(p => new ExportPatient
                {
                    Name = p.FullName,
                    AgeGroup = p.AgeGroup,
                    Gender = p.Gender.ToString().ToLower(),
                    Medicines = p.PatientsMedicines.Where(pm => pm.Medicine.ProductionDate > dateTime)
                    .OrderByDescending(pm => pm.Medicine.ExpiryDate)
                    .ThenBy(pm => pm.Medicine.Price)
                    .Select(m => new ExportPatientMedicine
                    {
                        Name = m.Medicine.Name,
                        Price = $"{m.Medicine.Price:f2}",
                        Producer = m.Medicine.Producer,
                        BestBefore = m.Medicine.ExpiryDate.ToString("yyyy-MM-dd"),
                        Category = ((Category)m.Medicine.Category).ToString().ToLower()
                    }).ToArray()
                }).ToArray();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportPatient[]), new XmlRootAttribute("Patients"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            bool omitXmlDeclaration = true; 
            XmlWriterSettings settings = new ()
            {
                OmitXmlDeclaration = omitXmlDeclaration,
                IndentChars = "\t",
                Indent = true,
                CheckCharacters = true,
            };
            
            StringBuilder xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(xml, settings))
            {
                xmlSerializer.Serialize(writer, patients, xsn);
            }
            return xml.ToString();
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .Where(m=>(int)m.Category == medicineCategory && m.Pharmacy.IsNonStop == true)
                .Select(m=> new ExportMedicines
                {
                    Name = m.Name,
                    Price = $"{ m.Price:f2}",
                    Pharmacy = new PharmacyDto
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber,
                    }
                }).ToList()
                .OrderBy(m=> m.Price)
                .ThenBy(m=>m.Name)
                .ToList();

            return JsonConvert.SerializeObject(medicines, Newtonsoft.Json.Formatting.Indented).Trim();
        }
    }
}
