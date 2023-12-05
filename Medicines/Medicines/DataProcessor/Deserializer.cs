namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.DataProcessor.ImportDtos;
    using Medicines.Extensions;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            List<ImportPatientsDto> patientsDto = jsonString
                .DeserializeFromJson<List<ImportPatientsDto>>();

            StringBuilder sb = new StringBuilder();

            foreach (var patient in patientsDto)
            {
                if (!IsValid(patient))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }             

                var patientToAdd = new Patient()
                {
                    FullName = patient.FullName,
                    AgeGroup = patient.AgeGroup,
                    Gender = patient.Gender,   
                };
                int patientMedicinesCount = 0;
                foreach (var medicine in patient.Medicines)
                {
                    if (!IsValid(medicine))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    if (patientToAdd.PatientsMedicines.Any(pm => pm.MedicineId == medicine))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    Medicine medicine1 = context.Medicines.Where(m => m.Id == medicine).FirstOrDefault();
                    PatientMedicine patientMedicine1 = new PatientMedicine
                    {
                        MedicineId = medicine,
                        PatientId = patientToAdd.Id
                    };
                    patientToAdd.PatientsMedicines.Add(patientMedicine1);
                    patientMedicinesCount++;
                    context.Patients.Add(patientToAdd);
                }

                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patientMedicinesCount));
            }

           

            return sb.ToString();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportPharmaciesDto> pharmaciesDto = xmlString
                .DeserializeFromXml<List<ImportPharmaciesDto>>("Pharmacies");

            List<Pharmacy> pharmacies = new List<Pharmacy>();

            foreach (var pharmacy in pharmaciesDto)
            {
                if (!IsValid(pharmacy))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }
               
                if (pharmacy.IsNonStop != "true" && pharmacy.IsNonStop != "false")
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }
                
                var pharmacyToAdd = new Pharmacy()
                {
                    Name = pharmacy.Name,
                    PhoneNumber = pharmacy.PhoneNumber,
                    IsNonStop = bool.Parse(pharmacy.IsNonStop)
                };

                foreach (var medicine in pharmacy.Medicines)
                {                                    
                    if (!IsValid(medicine))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    if (!IsValid(medicine.Category.ToString()))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(medicine.ProductionDate))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(medicine.ExpiryDate))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(medicine.Producer))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    if (DateTime.ParseExact(medicine.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) <=
                        DateTime.ParseExact(medicine.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                    
                    if (pharmacyToAdd.Medicines.Any(m => m.Name == medicine.Name && m.Producer == medicine.Producer))
                    {
                        var pharmacies1 = context.Pharmacies.Where(p => p.Medicines.Any(m => m.Name == medicine.Name && m.Producer == medicine.Producer));                      
                        if(!pharmacies1.Any()) 
                        {
                            sb.AppendLine(ErrorMessage);

                            continue;
                        }
                    }

                    pharmacyToAdd.Medicines.Add(new Medicine
                    {
                        Name = medicine.Name,
                        Price = medicine.Price,
                        ProductionDate = DateTime.ParseExact(medicine.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(medicine.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Producer = medicine.Producer,
                        Category = medicine.Category,
                    }) ; 
                }
                pharmacies.Add(pharmacyToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacyToAdd.Name, pharmacyToAdd.Medicines.Count()));
            }

            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();

            return sb.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
