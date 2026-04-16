using Hospital_System.Hospital_System;
using System;
using System.Collections.Generic;

namespace Hospital_System
{
    internal class Hospital
    {
        public string Name { get; set; } = "NeurAi Medical Center";
        public Floors[] Floors { get; private set; } = new Floors[5];
        public List<Department> ActiveDepartments { get; private set; } = new List<Department>();

        public ExternalDepartments CampusFacilities { get; set; }

        public Hospital(string name)
        {
            this.Name = name;

            for (int i = 0; i < 5; i++)
            {
                Floors[i] = new Floors(i + 1, $"Floor {i + 1}", true);
            }

            CampusFacilities = new ExternalDepartments(ambulanceCount: 5, bloodBankCapacity: 1000);

            // 2. Call the setup method when the hospital is built
            SetupAllDepartments();
        }

        // 3. The master method that builds every department
        private void SetupAllDepartments()
        {
            // --- 1. EMERGENCY (Floor 1) --->ENABLE THIS WHEN YOUR CLASS IS CREATED
          /*  EmergencyDepartment er = new EmergencyDepartment();
            er.AddRoom(101, Room.RoomType.Emergency, 1);
            er.AddRoom(102, Room.RoomType.ICU, 1);
            ActiveDepartments.Add(er);*/

            // --- 2. CARDIOLOGY (Floor 2) ---
            Department cardiology = new Department("Cardiology");
            cardiology.AddRoom(201, Room.RoomType.ICU, 2);
            cardiology.AddRoom(202, Room.RoomType.OperatingTheater, 2);
            cardiology.AddRoom(203, Room.RoomType.General, 2);
            ActiveDepartments.Add(cardiology);

            // --- 3. CHEST (Floor 3) ---
            Department chest = new Department("Chest");
            chest.AddRoom(301, Room.RoomType.Isolation, 3);
            chest.AddRoom(302, Room.RoomType.General, 3);
            ActiveDepartments.Add(chest);

            // --- 4. PEDIATRICS (Floor 4) ---
            Department pediatrics = new Department("Pediatrics");
            pediatrics.AddRoom(401, Room.RoomType.Incubator, 4);
            pediatrics.AddRoom(402, Room.RoomType.Maternity, 4);
            pediatrics.AddRoom(403, Room.RoomType.General, 4);
            ActiveDepartments.Add(pediatrics);

            // --- 5. GENERAL / PAINKILLERS (Floor 5) ---
            Department general = new Department("General");
            general.AddRoom(501, Room.RoomType.General, 5);
            general.AddRoom(502, Room.RoomType.General, 5);
            ActiveDepartments.Add(general);
        }
        public void ShowAllDepartments()
        {
            Console.WriteLine($"\n=== {Name} Departments ===");
            foreach (var dept in ActiveDepartments)
            {
                Console.WriteLine($"- {dept.DeptName} ({dept.Rooms.Count} Rooms)");
            }
        }
        public void ShowFloorDetails(int floorNumber)
        {
            if (floorNumber >= 1 && floorNumber <= 5)
            {
                Floors targetFloor = Floors[floorNumber - 1];
                Console.WriteLine($"\n=== Welcome to Floor {floorNumber} ===");
                Console.WriteLine($"Info: {targetFloor.Description}");
                Console.WriteLine($"Elevator Access: {(targetFloor.hasElevatorAccess ? "Yes" : "No")}");
                Console.WriteLine($"Capacity: Can add more rooms? {(targetFloor.CanAddMoreRooms() ? "Yes" : "No")}");

                Console.WriteLine("\nDepartments mapped to this floor level:");

                bool foundDepartments = false;
                foreach (var dept in ActiveDepartments)
                {
                    foreach (var room in dept.Rooms)
                    {
                        if (room.FloorNumber == floorNumber)
                        {
                            Console.WriteLine($"- {dept.DeptName} Department");
                            foundDepartments = true;
                            break;
                        }
                    }
                }

                if (!foundDepartments)
                {
                    Console.WriteLine("- No active clinical departments on this floor yet.");
                }
            }
            else
            {
                Console.WriteLine("[!] Invalid floor number. Please choose between 1 and 5.");
            }
        }
    }
}