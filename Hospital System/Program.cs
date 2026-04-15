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
/*
// Note: Replace MessageBox with Console.WriteLine if testing in Console
Console.WriteLine($"Room Number: {room1.RoomNumber}\nRoom Type: {room1.Type}\nFloor Number: {room1.FloorNumber}");
Console.WriteLine($"Room 1 Occupied: {room1.IsOccupied}");
Console.WriteLine($"Room 2 Occupied: {room2.IsOccupied}");
Room.ShowAvailableRooms();

*/

/*


Department surgicalWing = new Department("Surgical Wing");
for (int i = 302; i <= 305; i++)
{
    surgicalWing.Rooms.Add(new Room(i, Room.RoomType.General, 3));
}
Department emergency = new Department("Emergency");
for (int i = 100; i <= 110; i++)
{
    emergency.Rooms.Add(new Room(i, Room.RoomType.Emergency, 1));
}

Department pediatrics = new Department("Pediatrics & Neonatology");
for (int i = 201; i <= 205; i++)
{
    pediatrics.Rooms.Add(new Room(i, Room.RoomType.Maternity, 2));
}*/

//----------------------------------------------------------------


/*Department emergency2 = new Department("Emergency Department");

for (int i = 101; i <= 110; i++)
{
 
    Room.RoomType typeToAssign = (i % 5 == 0) ? Room.RoomType.ICU : Room.RoomType.General;

   
    if (i == 109) typeToAssign = Room.RoomType.Isolation;

    emergency.Rooms.Add(new Room(i, typeToAssign, 1));
}*/
bool stayInMenu = true;

while (stayInMenu)
{
    Console.WriteLine("\n--- NeurAi Medical Center Navigation ---");
    Console.WriteLine("1-5: View Specific Floor");
    Console.WriteLine("0:   View Entire Hospital (The big list)");
    Console.WriteLine("9:   Exit");
    Console.Write("Selection: ");

    string? input = Console.ReadLine();

    if (input == "9")
    {
        stayInMenu = false;
        return;
    }
    else if (input == "0")
    {
        neurai.DisplayHospitalMap(); 
    }
    else if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 5)
    {
        
        neurai.ShowFloorDetails(choice);
    }
    else
    {
        Console.WriteLine("Invalid selection. Please try again.");
    }
}

Department surgicalWing = new Department("Surgical Wing");


Department emergency = new Department("Emergency");

Department pediatrics = new Department("Pediatrics & Neonatology");


neurai.Floors[2].DepartmentsOnFloor.Add(surgicalWing); 
neurai.Floors[1].DepartmentsOnFloor.Add(pediatrics);   
neurai.Floors[0].DepartmentsOnFloor.Add(emergency);
neurai.DisplayHospitalMap();