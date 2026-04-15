using Hospital_System.Hospital_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Hospital
    {
        public string Name { get; set; }= "NeurAi";
        public Floors[] Floors { get; private set; } = new Floors[5];
        public ExternalDepartments facilities{ get; set; }
        public Hospital(string name)
        {
            this.Name = name;
            
            for (int i = 0; i < 5; i++)
            {
                Floors[i] = new Floors(i + 1, $"Floor {i + 1}", true);
            }
            facilities = new ExternalDepartments(ambulanceCount: 5, bloodBankCapacity: 1000); 
        }
        public void ShowFloorDetails(int choice)
        {
           
            int index = choice - 1;

            if (index >= 0 && index < Floors.Length)
            {
                var floor = Floors[index];
                Console.WriteLine($"\n--- DETAILED VIEW: {floor.Description} ---");

                foreach (var dept in floor.DepartmentsOnFloor)
                {
                    Console.WriteLine($"Department: {dept.DeptName}");
                    foreach (var room in dept.Rooms)
                    {
                        Console.WriteLine($"  - Room {room.RoomNumber} [{room.Type}]");
                    }
                }
            }
            else
            {
                Console.WriteLine("That floor doesn't exist!");
            }
        }
        public void DisplayHospitalMap()
        {
            Console.WriteLine($"--- {Name} Structure ---");
            foreach (var floor in Floors)
            {
                
                Console.WriteLine($"[Floor {floor.FloorNumber}] (Rooms {floor.FloorNumber}00-{floor.FloorNumber}99)");

                foreach (var dept in floor.DepartmentsOnFloor)
                {
                    Console.WriteLine($"  --> Department: {dept.DeptName}");
                    foreach (var room in dept.Rooms)
                    {
                        
                        Console.WriteLine($"      - Room {room.RoomNumber} [{room.Type}]");
                    }
                }
            }
        }
    }
}
