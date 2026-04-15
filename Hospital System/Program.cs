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
Department surgicalWing = new Department("Surgical Wing");


Department emergency = new Department("Emergency");

Department pediatrics = new Department("Pediatrics & Neonatology");
neurai.Floors[2].DepartmentsOnFloor.Add(surgicalWing);
neurai.Floors[1].DepartmentsOnFloor.Add(pediatrics);
neurai.Floors[0].DepartmentsOnFloor.Add(emergency);
bool stayInMenu = true;

while (stayInMenu)
{
    Console.WriteLine("\n--- NeurAi Medical Center Navigation ---");
    Console.WriteLine("1-5: View Specific Floor");
    Console.WriteLine("6:   View External Facilities");
    Console.WriteLine("0:   View Entire Hospital (The big list)");
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
            neurai.facilities.ShowExternalFacilities();
            Console.WriteLine("\nWould you like to open the Pharmacy menu? (Y/N)");
            string? pharmChoice = Console.ReadLine()?.ToUpper();
            if (pharmChoice == "Y")
            {
                neurai.facilities.HospitalPharmacy.ShowAll();
                Console.Write("\nEnter a medicine name to search (or press Enter to go back): ");
                string? searchName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    Medicine? foundMed = neurai.facilities.HospitalPharmacy.FindMedicine(searchName);
                    if (foundMed != null)
                    {
                        Console.WriteLine($"\n>>> Found Medicine: {foundMed.Name}, Price: {foundMed.Price:C}");
                    }
                    else
                    {
                        Console.WriteLine("\n>>> Medicine not found.");
                    }
                }
            }
            break;

        case "0":
            neurai.DisplayHospitalMap();
            neurai.facilities.ShowExternalFacilities();
            break;

        case "9":
            stayInMenu = false;
            Console.WriteLine("Exiting System. Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid selection. Please try again.");
            break;
    }
}
