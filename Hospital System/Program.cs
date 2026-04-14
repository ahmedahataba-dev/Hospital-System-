using System;
using System.Collections.Generic;

namespace Hospital_System
{
	internal class Program
	{
		// [STAThread] // You only need this if you are running a Windows Forms UI
		static void Main()
		{
			// ==========================================
			// Ahmed Hataba's Tests (Console Environment)
			// ==========================================
			Employee e1 = new Employee("ahmed", 19, GenderType.Male, "30701181200491", "01025920864", "ahmed@example.com", "Mansoura", 15000, 5); e1.CheckInandOut(e1);

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
		}
	}
}