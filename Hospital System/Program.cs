using Hospital_System;
using System;
using System.Collections.Generic;
// [STAThread] // You only need this if you are running a Windows Forms UI
// ==========================================
// Ahmed Hataba's Tests (Console Environment)
// ==========================================
Employee e1 = new Employee("Youssef", 19, GenderType.Male, "30704211200695", "01029611625", "youssef@example.com", "Mansoura", 15000, 5); e1.CheckInandOut(e1);

Console.WriteLine("\n-----------------------------------\n");

// ==========================================
// Youssef's Tests (Hospital & Rooms Environment)
// ==========================================

Hospital neurai = new Hospital("NeurAi Medical Center");
bool stayInMenu = true;
while (stayInMenu)
{
    Console.WriteLine("\n--- NeurAi Medical Center Navigation ---");
    Console.WriteLine("1-5: View Specific Floor");
    Console.WriteLine("6:   View Pharmacy & External Facilities");//Full pharmacy integration with search and department filtering
    Console.WriteLine("0:   View Entire Hospital");
    Console.WriteLine("9:   Exit");
    Console.Write("Selection: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
            neurai.ShowFloorDetails(int.Parse(choice));
            break;

        case "6":
            bool pharmacyMenu = true;
            neurai.CampusFacilities.ShowExternalFacilities();

            while (pharmacyMenu)
            {
                Console.WriteLine("\n=== Pharmacy Terminal ===");
                Console.WriteLine("1: View All Medicines in Hospital");
                Console.WriteLine("2: Search for a Specific Medicine");
                Console.WriteLine("3: View Medicines by Department");
                Console.WriteLine("0: Return to Main Menu");
                Console.Write("Pharmacy Selection: ");

                string? pharmChoice = Console.ReadLine();

                if (pharmChoice == "1")
                {
                    neurai.CampusFacilities.HospitalPharmacy.ShowAll();
                }
                else if (pharmChoice == "2")
                {
                    Console.Write("\nEnter a medicine name to search: ");
                    string? searchName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        Medicine? foundMed = neurai.CampusFacilities.HospitalPharmacy.FindMedicine(searchName);
                        if (foundMed != null)
                            Console.WriteLine($"\n>>> Found: {foundMed.Name}, Price: {foundMed.Price:C}");
                        else
                            Console.WriteLine("\n>>> Medicine not found.");
                    }
                }
                else if (pharmChoice == "3")
                {
                    Console.Write("\nEnter Department Name (Cardiology, Chest, Pediatrics, General): ");
                    string? targetDept = Console.ReadLine();

                    Department? foundDept = neurai.ActiveDepartments.Find(d =>
                        d.DeptName.Equals(targetDept, StringComparison.OrdinalIgnoreCase));

                    if (foundDept != null)
                    {
                        foundDept.ShowDepartmentMedicines(neurai.CampusFacilities.HospitalPharmacy);
                    }
                    else
                    {
                        Console.WriteLine($"[!] Could not find a department named '{targetDept}'.");
                    }
                }
                else if (pharmChoice == "0")
                {
                    pharmacyMenu = false;
                    Console.WriteLine("\nClosing Pharmacy Terminal...");
                }
                else
                {
                    Console.WriteLine("\n[!] Invalid selection. Please try again.");
                }
            }
            break;

        case "0":
            neurai.ShowAllDepartments();
            neurai.CampusFacilities.ShowExternalFacilities();
            break;

        case "9":
            stayInMenu = false;
            Console.WriteLine("Exiting System. Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid selection. Please try again.");
            break;
    }
    //-------------------------------------------------------------------\\
}
