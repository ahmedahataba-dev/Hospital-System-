
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
        public SecurityDepartment CampusSecurity { get; private set; }
        public Hospital(string name)
        {
            CampusSecurity = new SecurityDepartment();
            CampusSecurity.HireGuard(new Security("Ahmed", "SEC-001", "Day"));
            CampusSecurity.HireGuard(new Security("Mohammed", "SEC-002", "Evening"));
            CampusSecurity.HireGuard(new Security("Adham", "SEC-003", "Night"));
            this.Name = name;

            for (int i = 0; i < 5; i++)
            {
                Floors[i] = new Floors(i + 1, $"Floor {i + 1}", true);
            }

            CampusFacilities = new ExternalDepartments(ambulanceCount: 5, bloodBankCapacity: 1000);
            SetupAllDepartments();
        }
        private void SetupAllDepartments()
        {
            ActiveDepartments.Clear();
            ActiveDepartments.Add(new Department("General Medicine"));
            ActiveDepartments.Add(new Department("Internal Medicine"));
            ActiveDepartments.Add(new Department("Neurology"));
            ActiveDepartments.Add(new Department("Ophthalmology"));
            ActiveDepartments.Add(new Department("Physical Medicine"));
            ActiveDepartments.Add(new Department("Otolaryngology (ENT)"));
            ActiveDepartments.Add(new Department("Cardiology"));
            ActiveDepartments.Add(new Department("Orthopedics"));
            ActiveDepartments.Add(new Department("Pulmonology"));
            ActiveDepartments.Add(new Department("Dermatology"));
            ActiveDepartments.Add(new Department("General Surgery"));


            // --- ROOM DISTRIBUTION SYSTEM  ---
            int currentFloor = 1;
            int roomsOnCurrentFloor = 0;
            int deptIndex = 0;
            int roomsGivenToCurrentDept = 0;
            for (int i = 1; i <= 100; i++)
            {
                int roomNum = (currentFloor * 100) + (roomsOnCurrentFloor + 1);
                string roomType = "General";
                if (i % 10 == 0) roomType = "ICU";
                else if (i % 15 == 0) roomType = "OperatingTheater";
                ActiveDepartments[deptIndex].Rooms.Add(new Rooms(roomNum, Enum.TryParse<Rooms.RoomType>(roomType, out var parsedRoomType) ? parsedRoomType : Rooms.RoomType.General, currentFloor));
                //Console.WriteLine($"[+] Room {roomNum} ({roomType}) added to {ActiveDepartments[deptIndex].DeptName}.");

                roomsGivenToCurrentDept++;
                if (roomsGivenToCurrentDept >= 9 && deptIndex < ActiveDepartments.Count - 1)
                {
                    deptIndex++;
                    roomsGivenToCurrentDept = 0;
                }
                roomsOnCurrentFloor++;
                if (roomsOnCurrentFloor == 20)
                {
                    currentFloor++;
                    roomsOnCurrentFloor = 0;
                }
            }

        }
        public void ShowAllDepartments()
        {
            // Console.WriteLine($"\n=== {Name} Departments ===");
            foreach (var dept in ActiveDepartments)
            {
                // Console.WriteLine($"- {dept.DeptName} ({dept.Rooms.Count} Rooms)");
            }
        }
        public void ShowFloorDetails(int floorNumber)
        {
            if (floorNumber >= 1 && floorNumber <= 5)
            {
                Floors targetFloor = Floors[floorNumber - 1];
                // Console.WriteLine($"\n=== Welcome to Floor {floorNumber} ===");
                // Console.WriteLine($"Info: {targetFloor.Description}");
                // Console.WriteLine($"Elevator Access: {(targetFloor.hasElevatorAccess ? "Yes" : "No")}");
                // Console.WriteLine($"Capacity: Can add more rooms? {(targetFloor.CanAddMoreRooms() ? "Yes" : "No")}");

                // Console.WriteLine("\nDepartments mapped to this floor level:");

                bool foundDepartments = false;
                foreach (var dept in ActiveDepartments)
                {
                    foreach (var room in dept.Rooms)
                    {
                        if (room.FloorNumber == floorNumber)
                        {
                            // Console.WriteLine($"- {dept.DeptName} Department");
                            foundDepartments = true;
                            break;
                        }
                    }
                }

                if (!foundDepartments)
                {
                    // Console.WriteLine("- No active clinical departments on this floor yet.");
                }
            }
            else
            {
                // Console.WriteLine("[!] Invalid floor number. Please choose between 1 and 5.");
            }
        }



        protected internal static void RunMainMenu(Hospital neurai)
        {
            // This console menu is disabled in the Windows Forms application.
            // All navigation is handled through the MainForm UI.
        }
    }
}