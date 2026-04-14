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
/*
Hospital neurai = new Hospital();
Floors floor1 = new Floors(1, "Main Floor", true);
Department cardiology = new Department("Cardiology");
Room heartRoom = new Room(101, Room.RoomType.ICU, 1);

cardiology.Rooms.Add(heartRoom);
floor1.DepartmentsOnFloor.Add(cardiology);
neurai.AddFloor(floor1);

Room room1 = new Room(201, Room.RoomType.ICU, 2);
Room room2 = new Room(207, Room.RoomType.ICU, 2);

// Note: Replace MessageBox with Console.WriteLine if testing in Console
Console.WriteLine($"Room Number: {room1.RoomNumber}\nRoom Type: {room1.Type}\nFloor Number: {room1.FloorNumber}");
Console.WriteLine($"Room 1 Occupied: {room1.IsOccupied}");
Console.WriteLine($"Room 2 Occupied: {room2.IsOccupied}");
Room.ShowAvailableRooms();
*/

Hospital neurAi = new Hospital("NeurAi Medical Center");


Department surgicalWing = new Department("Surgical Wing");
for (int i = 102; i <= 105; i++)
{
    surgicalWing.Rooms.Add(new Room(i, Room.RoomType.General, 3));
}

Department pediatrics = new Department("Pediatrics & Neonatology");
for (int i = 201; i <= 205; i++)
{
    pediatrics.Rooms.Add(new Room(i, Room.RoomType.Maternity, 2));
}
neurAi.Floors[0].DepartmentsOnFloor.Add(surgicalWing); 
neurAi.Floors[1].DepartmentsOnFloor.Add(pediatrics);   
neurAi.DisplayHospitalMap();
