using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static Hospital_System.program;

namespace Hospital_System
{
    internal class HospitalEngine
    {
        public static List<Patient> patients = new List<Patient>();
        public static List<Operation> operationsList = new List<Operation>();
        public static BloodBank myBank = new BloodBank();
        public static bool[] roomsStatus = new bool[9];
        public const string filePath = "patients_data.json";
        public const string operationsFilePath = "operations_data.json";

        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        // ===================== SAVE =====================
        public void SaveData()
        {
            try
            {
                string patientsJson = JsonConvert.SerializeObject(patients, jsonSettings);
                File.WriteAllText(filePath, patientsJson);

                string operationsJson = JsonConvert.SerializeObject(operationsList, jsonSettings);
                File.WriteAllText(operationsFilePath, operationsJson);

                string roomsJson = JsonConvert.SerializeObject(roomsStatus, Formatting.Indented);
                File.WriteAllText("rooms_data.json", roomsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

       
        public void LoadData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    patients = JsonConvert.DeserializeObject<List<Patient>>(jsonData, jsonSettings) ?? new List<Patient>();
                }

                if (File.Exists(operationsFilePath))
                {
                    string opData = File.ReadAllText(operationsFilePath);
                    operationsList = JsonConvert.DeserializeObject<List<Operation>>(opData, jsonSettings) ?? new List<Operation>();
                }

                if (File.Exists("rooms_data.json"))
                {
                    string roomsData = File.ReadAllText("rooms_data.json");
                    var loadedRooms = JsonConvert.DeserializeObject<bool[]>(roomsData);
                    if (loadedRooms != null) roomsStatus = loadedRooms;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

       
        public void AddInpatient()
        {
            Inpatient p = new Inpatient();
            FillBasicInfo(p);
            p.WardName = InputHelper.ReadString("Ward Name: ");
            p.RoomNumber = InputHelper.ReadInt("Room Number: ");
            p.BedNumber = InputHelper.ReadInt("Bed Number: ");
            p.AttendingPhysician = InputHelper.ReadString("Attending Physician: ");
            p.DietPlan = InputHelper.ReadString("Diet Plan: ");
            Console.Write("Has Operation? (y/n): ");
            p.HasOperations = Console.ReadLine().ToLower() == "y";
            if (p.HasOperations) p.OperationType = InputHelper.ReadString("Operation Type: ");

            patients.Add(p);
            SaveData(); 
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }

        
        public void AddOutpatient()
        {
            Outpatient p = new Outpatient();
            FillBasicInfo(p);
            p.ClinicName = InputHelper.ReadString("Clinic Name: ");
            p.ConsultationFee = InputHelper.ReadDouble("Fee: ");
            p.Complaint = InputHelper.ReadString("Complaint: ");

            patients.Add(p);
            SaveData(); 
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }

        
        public void ViewByCategory()
        {
            Console.WriteLine("\n1. View Inpatients | 2. View Outpatients | 3. View All");
            Console.Write("Select choice: ");
            string view = Console.ReadLine();

            if (view == "1")
                patients.OfType<Inpatient>().ToList().ForEach(p => p.Display());
            else if (view == "2")
                patients.OfType<Outpatient>().ToList().ForEach(p => p.Display());
            else
                patients.ForEach(p => p.Display());

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        
        public void Search()
        {
            string query = InputHelper.ReadString("Enter Name or National ID to search: ").ToLower();
            var results = patients.Where(p =>
                p.Name.ToLower().Contains(query) ||
                p.NationalId.Contains(query)).ToList();

            if (results.Any())
                results.ForEach(p => p.Display());
            else
                Console.WriteLine("No patient found with these details.");
            Console.ReadKey();
        }

        
        public void UpdatePatient()
        {
            Console.Write("Enter Patient ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var p = patients.FirstOrDefault(x => x.PatientId == id);
                if (p != null)
                {
                    Console.WriteLine("--- Update Menu (Leave empty to keep current value) ---");

                    Console.Write($"Name [{p.Name}]: ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name)) p.Name = name;

                    Console.Write($"Age [{p.Age}]: ");
                    string ageInput = Console.ReadLine();
                    if (int.TryParse(ageInput, out int age)) p.Age = age;

                    Console.Write($"Medical Case [{p.MedicalCase}]: ");
                    string mCase = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(mCase)) p.MedicalCase = mCase;

                    SaveData(); 
                    Console.WriteLine("Updated successfully!");
                }
                else Console.WriteLine("ID not found.");
            }
            Console.ReadKey();
        }

       
        public void DeletePatient()
        {
            Console.Write("Enter the ID of the patient to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var patient = patients.FirstOrDefault(p => p.PatientId == id);
                if (patient != null)
                {
                    patients.Remove(patient);
                    SaveData(); 
                    Console.WriteLine("Patient record deleted successfully.");
                    Console.ReadKey();
                }
                else Console.WriteLine("Patient ID not found.");
            }
        }

      
        public void FillBasicInfo(Patient p)
        {
            p.PatientId = patients.Count == 0 ? 1 : patients.Max(x => x.PatientId) + 1;
            Console.WriteLine($"\n[Patient ID: {p.PatientId}]");

            p.Name = InputHelper.ReadLettersOnly("Enter Name: ");

            while (true)
            {
                try
                {
                    p.Age = InputHelper.ReadInt("Enter Age: ");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Select Gender: 1. Male | 2. Female");
            string gChoice = Console.ReadLine();
            p.Gender = (gChoice == "2") ? GenderType.Female : GenderType.Male;

            while (true)
            {
                try
                {
                    p.NationalId = InputHelper.ReadString("National ID (14 digits): ");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            while (true)
            {
                try
                {
                    p.PhoneNumber = InputHelper.ReadString("Phone Number (11 digits): ");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            p.Height = InputHelper.ReadDouble("Height (m): ");
            p.Weight = InputHelper.ReadDouble("Weight (kg): ");
            p.BloodType = ReadBloodType();
            p.MedicalCase = InputHelper.ReadString("Medical Case: ");
            p.Allergies = InputHelper.ReadString("Allergies: ");
            p.Risk = InputHelper.ReadString("Risk Level: ");
            p.MedicalHistory = InputHelper.ReadString("Medical History: ");
            Console.Write("Last Surgery Date (yyyy-mm-dd) or Enter to skip: ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime last)) p.LastSurgeryDate = last;

            Console.Write("Next Surgery Date (yyyy-mm-dd) or Enter to skip: ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime next)) p.NextScheduledProcedure = next;

            p.PreviousSurgeriesLog = InputHelper.ReadString("Full Surgical History: ");
            p.FamilyHistory = InputHelper.ReadString("Family History: ");
            p.PaymentMethods = InputHelper.ReadString("Payment Method: ");
        }

        public static BloodGroup ReadBloodType()
        {
            while (true)
            {
                Console.WriteLine("\nChoose your blood type:");
                var types = Enum.GetValues(typeof(BloodGroup));
                for (int i = 0; i < types.Length; i++)
                    Console.WriteLine($"{i + 1}. {types.GetValue(i)}");

                Console.Write("Choose from (1-8): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 8)
                    return (BloodGroup)(choice - 1);

                Console.WriteLine("Wrong number!");
            }
        }
    }
}