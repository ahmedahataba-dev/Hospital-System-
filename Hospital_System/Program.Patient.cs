using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_System
{
    public partial class program
    {
        // إضافة مريض محجوز في المستشفى
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
            SaveData();
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }
        //  إضافة مريض عيادة خارجية يعني هيكشف وماشي
        static void AddOutpatient()
        {
            Outpatient p = new Outpatient();
            FillBasicInfo(p);
            p.ClinicName = ReadString("Clinic Name: ");
            p.ConsultationFee = ReadDouble("Fee: ");
            p.Complaint = ReadString("Complaint: ");

            patients.Add(p);
            SaveData();
            Console.WriteLine("\nSaved! Press any key...");
            Console.ReadKey();
        }
        //  عرض المرضى 
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
        // بحث بالرقم القومي او الاسم
        static void Search()
        {
            string query = ReadString("Enter Name or National ID to search: ").ToLower();
            var results = patients.Where(p =>
                p.Name.ToLower().Contains(query) ||
                p.NationalId.Contains(query)).ToList();

            if (results.Any())
                results.ForEach(p => p.Display());
            else
                Console.WriteLine("No patient found with these details.");
            Console.ReadKey();
        }
        // عشان لو عايزه اعدل على حاجه , بس غالبا هشيلها
        static void UpdatePatient()
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
        // مسح مريض من السيستم
        static void DeletePatient()
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
        // عشان البيانات المشتركة بين المرضى كلهم
        static void FillBasicInfo(Patient p)
        {
            //عشان يدي ID تلقائي للمريض
            p.PatientId = patients.Count == 0 ? 1 : patients.Max(x => x.PatientId) + 1;
            Console.WriteLine($"\n[Patient ID: {p.PatientId}]");


            p.Name = ReadLettersOnly("Enter Name: ");


            while (true)
            {
                try
                {
                    p.Age = ReadInt("Enter Age: ");
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


            while (true) // عشان يدخل 14 رقم في الرقم القومي
            {
                try
                {
                    p.NationalId = ReadString("National ID (14 digits): ");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }


            while (true) //عشان يدخل 11 رقم في رقم التليفون
            {
                try
                {
                    p.PhoneNumber = ReadString("Phone Number (11 digits): ");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            p.Height = ReadDouble("Height (m): ");
            p.Weight = ReadDouble("Weight (kg): ");
            p.BloodType = ReadBloodType();
            p.MedicalCase = ReadString("Medical Case: ");
            p.Allergies = ReadString("Allergies: ");
            p.Risk = ReadString("Risk Level: ");
            p.MedicalHistory = ReadString("Medical History: ");
             Console.Write("Last Surgery Date (yyyy-mm-dd) or Enter to skip: ");
             if (DateTime.TryParse(Console.ReadLine(), out DateTime last)) p.LastSurgeryDate = last;

             Console.Write("Next Surgery Date (yyyy-mm-dd) or Enter to skip: ");
             if (DateTime.TryParse(Console.ReadLine(), out DateTime next)) p.NextScheduledProcedure = next;

            p.PreviousSurgeriesLog = ReadString("Full Surgical History: ");
            p.FamilyHistory = ReadString("Family History: ");
            p.PaymentMethods = ReadString("Payment Method: ");
        }

    }
}
