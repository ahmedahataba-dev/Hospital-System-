using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace Hospital_System
{
    

    // enum for blood type
    public enum BloodGroup
    {

        A_Positive, A_Negative,
        B_Positive, B_Negative,
        AB_Positive, AB_Negative,
        O_Positive, O_Negative
    }

    public partial class program
    {
        static List<Patient> patients = new List<Patient>();
        static List<Operation> operationsList = new List<Operation>();

        const string filePath = "patients_data.json"; // الملف اللي هيتحفظ فيه ال Health Record
        const string operationsFilePath = "operations_data.json";

        //saving (inpatient / outpatient)
        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All, // عشان يحفظ النوعين inpatient/outpatient 
            Formatting = Formatting.Indented
        };

        static BloodBank myBank = new BloodBank();

        // دي مصفوفة بتمثل 5 غرف عمليات، لو false تبقى فاضية، لو true تبقى محجوزة
        static bool[] roomsStatus = new bool[9];

        static void Main(string[] args)
        {
            
            LoadData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("     HOSPITAL MANAGEMENT SYSTEM     ");
                Console.WriteLine("\n1. Add Inpatient");
                Console.WriteLine("2. Add Outpatient");
                Console.WriteLine("3. View All Patients");
                Console.WriteLine("4. Search for Patient (Name/National ID)");
                Console.WriteLine("5. Update Patient Data");
                Console.WriteLine("6. Delete Patient");
                Console.WriteLine("7. Blood Bank System Menu");
                Console.WriteLine("8. Operating System Menu");
                Console.WriteLine("9. Exit");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddInpatient(); break;
                    case "2": AddOutpatient(); break;
                    case "3": ViewByCategory(); break;
                    case "4": Search(); break;
                    case "5": UpdatePatient(); break;
                    case "6": DeletePatient(); break;
                    case "7": BloodBankMenu(); break;
                    case "8": OperationMenu(); break;
                    case "9":
                        SaveData();
                        Console.WriteLine("Exiting... Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        //  حفظ الداتا في ملفات JSON
        static void SaveData()
        {
            try
            {

                string patientsJson = JsonConvert.SerializeObject(patients, jsonSettings);
                File.WriteAllText(filePath, patientsJson);

                string operationsJson = JsonConvert.SerializeObject(operationsList, jsonSettings);
                File.WriteAllText(operationsFilePath, operationsJson);


                Console.WriteLine("\n[System: All data saved successfully]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }
        // تحميل الداتا من الملفات لما البرنامج يبدأ
        static void LoadData()
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

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
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




















































    }
}



























































