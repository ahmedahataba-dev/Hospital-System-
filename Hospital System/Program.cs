using Hospital_System;
using System;
using System.Numerics;
using System;

namespace Hospital_System
{
    using System;



    namespace Hospital_System
    {
        class Program
        {
            static void Main(string[] args)
            {
                Pharmacy pharmacy = new Pharmacy();

                List<Prescription> prescriptions = new List<Prescription>();

                Console.WriteLine("===== HOSPITAL SYSTEM =====");

                // 👨‍⚕️ Doctor
                string doctor;
                do
                {
                    Console.Write("Enter Doctor Name: ");
                    doctor = Console.ReadLine();
                }
                while (string.IsNullOrWhiteSpace(doctor));

                // 👤 Patient (اختياري - بدون كلاس Patient)
                string patientName;

                Console.Write("Enter Patient Name: ");
                patientName = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(patientName))
                {
                    Console.Write("Invalid  Enter again: ");
                    patientName = Console.ReadLine();
                }

                // 🏥 Department
                string dept;

                while (true)
                {
                    pharmacy.ShowAll();

                    Console.Write("Choose Department Number: ");

                    if (!int.TryParse(Console.ReadLine(), out int index))
                    {
                        Console.WriteLine("Invalid input ");
                        continue;
                    }

                    dept = pharmacy.GetCategoryNameByIndex(index);

                    if (dept != null)
                    {
                        Console.WriteLine("Department Selected ");
                        break;
                    }

                    Console.WriteLine("Invalid Department  Try again");
                }

                //  MENU
                while (true)
                {
                    Console.WriteLine("\n========== MENU ==========");
                    Console.WriteLine("1. Show All Categories");
                    Console.WriteLine("2. Show Medicines in Category");
                    Console.WriteLine("3. Search Medicine");
                    Console.WriteLine("4. Create Prescription");
                    Console.WriteLine("5. Show Prescriptions");
                    Console.WriteLine("6. Exit");
                    Console.WriteLine("==========================");

                    Console.Write("Choose option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        Console.WriteLine("Enter number only ");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            pharmacy.ShowAll();
                            break;

                        case 2:
                            pharmacy.ShowAll();

                            Console.Write("Enter Category Number: ");
                            if (!int.TryParse(Console.ReadLine(), out int catIndex))
                            {
                                Console.WriteLine("Invalid ");
                                break;
                            }

                            string cat = pharmacy.GetCategoryNameByIndex(catIndex);

                            if (cat == null)
                            {
                                Console.WriteLine("Invalid Category ");
                                break;
                            }

                            pharmacy.ShowCategory(cat);
                            break;

                        case 3:
                            Console.Write("Enter Medicine Name: ");
                            string search = Console.ReadLine();

                            var found = pharmacy.FindMedicine(search);

                            if (found != null)
                                Console.WriteLine(found.ToString());
                            else
                                Console.WriteLine("Medicine Not Found ");

                            break;

                        case 4:
                            Prescription prescription = new Prescription();
                            prescription.DoctorName = doctor;
                            prescription.PatientName = patientName;
                            prescription.Department = dept;

                            pharmacy.ShowAll();

                            Console.Write("Enter Category Number: ");
                            if (!int.TryParse(Console.ReadLine(), out int catIndex4))
                            {
                                Console.WriteLine("Invalid ");
                                break;
                            }

                            string cat4 = pharmacy.GetCategoryNameByIndex(catIndex4);

                            if (cat4 == null)
                            {
                                Console.WriteLine("Invalid Category ");
                                break;
                            }

                            var medicines = pharmacy.categories[cat4];

                            Console.Write("How many medicines ");
                            if (!int.TryParse(Console.ReadLine(), out int count))
                            {
                                Console.WriteLine("Invalid ");
                                break;
                            }

                            for (int i = 0; i < count; i++)
                            {
                                Console.WriteLine("\n--- Medicines List ---");

                                for (int j = 0; j < medicines.Count; j++)
                                {
                                    Console.WriteLine($"{j + 1}. {medicines[j].Name} | Stock: {medicines[j].Quantity}");
                                }

                                Console.Write("Choose Medicine Number: ");

                                if (!int.TryParse(Console.ReadLine(), out int medChoice) ||
                                    medChoice < 1 || medChoice > medicines.Count)
                                {
                                    Console.WriteLine("Invalid ");
                                    i--;
                                    continue;
                                }

                                Medicine med = medicines[medChoice - 1];

                                Console.Write("Enter Quantity: ");
                                int qty = int.Parse(Console.ReadLine());

                                if (qty > med.Quantity)
                                {
                                    Console.WriteLine("Not enough stock ");
                                    i--;
                                    continue;
                                }

                                Console.Write("Times Per Day: ");
                                int times = int.Parse(Console.ReadLine());

                                Console.Write("Number of Days: ");
                                int days = int.Parse(Console.ReadLine());

                                prescription.AddMedicine(med, qty, times, days);

                                Console.WriteLine("Added ");
                            }

                            prescriptions.Add(prescription);

                            Console.WriteLine("\nPrescription Created Successfully ");
                            break;

                        case 5:
                            if (prescriptions.Count == 0)
                            {
                                Console.WriteLine("No prescriptions ");
                                break;
                            }

                            foreach (var p in prescriptions)
                                p.ShowPrescription();

                            break;

                        case 6:
                            Console.WriteLine("Exiting...");
                            return;

                        default:
                            Console.WriteLine("Invalid Choice ");
                            break;
                    }
                }
            }
        }
    }
}