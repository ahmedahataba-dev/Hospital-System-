using Hospital_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Hospital_System
{
   
    public enum BloodGroup
    {

        A_Positive, A_Negative,
        B_Positive, B_Negative,
        AB_Positive, AB_Negative,
        O_Positive, O_Negative
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
                Console.WriteLine("      Hospital Management System    ");
                Console.ResetColor();
                Console.WriteLine("1. Add Inpatient ");
                Console.WriteLine("2. Add Outpatient ");
                Console.WriteLine("3. View Patients(by category)");
                Console.WriteLine("4. Search ID");
                Console.WriteLine("5. Exit");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddInpatient();
                        break;
                    case "2":
                        AddOutpatient();
                        break;
                    case "3":
                        ViewByCategory();
                        break;
                    case "4":
                        SearchByCategory();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static string ReadString(string empty)
        {
            while (true)
            {
                Console.Write(empty);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("Error! This field cannot be empty.");
            }
        }

        static string ReadLettersOnly(string letters)
        {
            while (true)
            {
                Console.Write(letters);
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

                if (input == "y")
                    return true;
                if (input == "n")
                    return false;

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

            p.NationalId=ReadString("NationalId:")
           p.PhoneNumber = ReadString ("Phone number");
            p.Height = ReadDouble("Height (m): ");
            p.Weight = ReadDouble("Weight (kg): ");
            p.BloodType = ReadBloodType();
            p.MedicalCase = ReadString("Medical Case: ");
            p.Allergies = ReadString("Allergies: ");
            p.Risk = ReadString("Risk Level: ");
            p.MedicalHistory = ReadString("Medical History: ");
            p.FamilyHistory = ReadString("Family History:");
            p.PaymentMethods = ReadString("Payment Method: ");


        }

        static void AddInpatient()
        {
            Inpatient p = new Inpatient();
            FillBasicInfo(p);
            p.WardName = ReadString("Ward Name: ");
            p.RoomNumber = ReadInt("Room Number: ");
            p.BedNumber = ReadInt("Bed Number: ");
            p.AttendingPhysician = ReadString("Attending Physician: ");
            p.DietPlan = ReadString("Diet Plan: ");
            Console.Write("Has Operation? (y/n): ");
            p.HasOperations = Console.ReadLine().ToLower() == "y";
            if (p.HasOperations) p.OperationType = ReadString("Operation Type: ");

            patients.Add(p);
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }


        static void AddOutpatient()
        {
            Outpatient p = new Outpatient();
            FillBasicInfo(p);
            p.ClinicName = ReadString("Clinic Name: ");
            p.ConsultationFee = ReadDouble("Fee: ");
            p.Complaint = ReadString("Complaint: ");

            patients.Add(p);
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }

        static void ViewByCategory()
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

        static void SearchByCategory()
        {
            Console.WriteLine("\nSearch in: 1. Inpatients | 2. Outpatients");
            Console.Write("Choice: ");
            string search = Console.ReadLine();

            int id = ReadInt("Enter ID to search: ");
            Patient found = null;

            if (search == "1") found = patients.OfType<Inpatient>().FirstOrDefault(x => x.PatientId == id);
            else if (search == "2") found = patients.OfType<Outpatient>().FirstOrDefault(x => x.PatientId == id);

            if (found != null) found.Display();

            else Console.WriteLine("Not found in this category.");

            Console.ReadKey();
        }

        static int ReadInt(string Int)
        {
            while (true)
            {
                Console.Write(Int);

                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;

                Console.WriteLine("Error! Please enter a valid number.");
            }
        }

        static double ReadDouble(string Double)
        {
            while (true)
            {
                Console.Write(Double);
                if (double.TryParse(Console.ReadLine(), out double result))
                    return result;
                Console.WriteLine("Error! Please enter a valid decimal number.");
            }
        }

    }
    }

