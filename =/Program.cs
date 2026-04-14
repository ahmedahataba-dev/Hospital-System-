using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HospitalSystem
{
    public enum BloodGroup
    {
        A_Positive, A_Negative,
        B_Positive, B_Negative,
        AB_Positive, AB_Negative,
        O_Positive, O_Negative
    }

    class Patient : Person
    {
        public int PatientId { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Allergies { get; set; }
        public string MedicalCase { get; set; }
        public BloodGroup BloodType { get; set; }
        public string MedicalHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string Risk { get; set; }
        public string PaymentMethods { get; set; }

        public virtual void Display()
        {
            Console.WriteLine($"\n[ID: {PatientId}] | Name: {Name} | Age: {Age} | Gender: {Gender}");
            Console.WriteLine($"National ID: {NationalId} | Phone: {PhoneNumber}");
            Console.WriteLine($"Case: {MedicalCase} | Risk: {Risk} | Blood: {BloodType}");
            Console.WriteLine($"Height: {Height}m | Weight: {Weight}kg");
            Console.WriteLine($"Allergies: {Allergies} | Payment: {PaymentMethods}");
            Console.WriteLine($"Medical History: {MedicalHistory}");
            Console.WriteLine($"Family History: {FamilyHistory}");
        }
    }

    class Inpatient : Patient
    {
        public string WardName { get; set; }
        public int RoomNumber { get; set; }
        public int BedNumber { get; set; }
        public bool HasOperations { get; set; }
        public string OperationType { get; set; }
        public string Medication { get; set; }
        public string AttendingPhysician { get; set; }
        public string DietPlan { get; set; }

        public override void Display()
        {
            base.Display();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("      Inpatient Specific Info      ");
            Console.ResetColor();
            Console.WriteLine($"Ward: {WardName} | Room: {RoomNumber} | Bed: {BedNumber}");
            Console.WriteLine($"Physician: {AttendingPhysician} | Diet: {DietPlan}");
            if (HasOperations) Console.WriteLine($"Operation: {OperationType}");
            Console.WriteLine("--------------------------------------------------");
        }
    }

    class Outpatient : Patient
    {
        public string ClinicName { get; set; }
        public double ConsultationFee { get; set; }
        public string ChiefComplaint { get; set; }

        public override void Display()
        {
            base.Display();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("      Outpatient Specific Info      ");
            Console.ResetColor();
            Console.WriteLine($"Clinic: {ClinicName} | Fee: {ConsultationFee} EGP");
            Console.WriteLine($"Complaint: {ChiefComplaint}");
            Console.WriteLine("--------------------------------------------------");
        }
    }

    class Program
    {
        static List<Patient> patients = new List<Patient>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("      WELCOME!      ");
                Console.ResetColor();
                Console.WriteLine("1. Add Inpatient ");
                Console.WriteLine("2. Add Outpatient ");
                Console.WriteLine("3. View Patients");
                Console.WriteLine("4. Search ID");
                Console.WriteLine("5. Exit");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddInpatient(); break;
                    case "2": AddOutpatient(); break;
                    case "3": ViewByCategory(); break;
                    case "4": SearchByCategory(); break;
                    case "5": return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input;
                Console.WriteLine("Error! This field cannot be empty.");
            }
        }

        static string ReadLettersOnly(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return input;
                Console.WriteLine("Error! Only letters and spaces are allowed.");
            }
        }

        static BloodGroup ReadBloodType()
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

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " (y/n): ");
                string input = (Console.ReadLine() ?? "").ToLower();

                if (input == "y") return true;
                if (input == "n") return false;

                Console.WriteLine("Invalid input! Please enter 'y' for Yes or 'n' for No.");
            }
        }

        static void FillBasicInfo(Patient p)
        {
            p.PatientId = patients.Count == 0 ? 1 : patients.Max(x => x.PatientId) + 1;
            Console.WriteLine($"\n[Patient ID Generated Automatically: {p.PatientId}]");

            try { p.Name = ReadLettersOnly("Enter Name: "); } catch (Exception ex) { Console.WriteLine(ex.Message); }

            try
            {
                Console.Write("Enter Age: ");
                p.Age = int.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            p.Height = ReadDouble("Height (meters): ");
            p.Weight = ReadDouble("Weight (kg): ");
            p.BloodType = ReadBloodType();
            p.MedicalCase = ReadString("Medical Case: ");
            p.Allergies = ReadString("Allergies: ");
            p.Risk = ReadString("Risk Level: ");
            p.PaymentMethods = ReadString("Payment Method: ");
        }

        static void AddInpatient()
        {
            Console.WriteLine("\n--- Add New Inpatient ---");
            Inpatient p = new Inpatient();
            FillBasicInfo(p);

            p.WardName = ReadString("Ward Name: ");
            p.RoomNumber = ReadInt("Room Number: ");
            p.AttendingPhysician = ReadString("Attending Physician: ");
            p.HasOperations = ReadBool("Does the patient have surgery?");

            if (p.HasOperations)
                p.OperationType = ReadString("Operation Type: ");

            patients.Add(p);
            Console.WriteLine("\nPatient data saved successfully!");
            Console.ReadKey();
        }

        static void ViewByCategory()
        {
            Console.WriteLine("\n1. View Inpatients | 2. View Outpatients | 3. View All");
            Console.Write("Select choice: ");
            string sub = Console.ReadLine();

            if (sub == "1")
                patients.OfType<Inpatient>().ToList().ForEach(p => p.Display());
            else if (sub == "2")
                patients.OfType<Outpatient>().ToList().ForEach(p => p.Display());
            else
                patients.ForEach(p => p.Display());

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void SearchByCategory()
        {
            Console.Write("\nEnter Patient ID to search: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var patient = patients.FirstOrDefault(p => p.PatientId == id);
                if (patient != null)
                {
                    patient.Display();
                }
                else
                {
                    Console.WriteLine("Patient not found!");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format!");
            }
            Console.ReadKey();
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result)) return result;
                Console.WriteLine("Error! Please enter a valid number.");
            }
        }

        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double result)) return result;
                Console.WriteLine("Error! Please enter a valid decimal number.");
            }
        }
    }
}