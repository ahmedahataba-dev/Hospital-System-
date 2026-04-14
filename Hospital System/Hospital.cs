using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Hospital
    {
        public string Name { get; set; }= "NeurAi";
        public Floors[] Floors { get; private set; } = new Floors[5];
        public Hospital(string name)
        {
            this.Name = name;
            
            for (int i = 0; i < 5; i++)
            {
                Floors[i] = new Floors(i + 1, $"Floor {i + 1}", true);
            }
        }
        public void DisplayHospitalMap()
        {
            Console.WriteLine($"--- {Name} Structure ---");

            // Use 'floor' (singular) for the loop variable
            foreach (var floor in Floors)
            {
                // 1. Fixed: Changed 'floors' to 'floor'
                Console.WriteLine($"[Floor {floor.FloorNumber}] - {floor.Description}");

                // 2. Ensure your Floor class has a 'List<Department> Departments'
                foreach (var dept in floor.DepartmentsOnFloor)
                {
                    Console.WriteLine($"  --> Department: {dept.DeptName} ({dept.Rooms.Count} Rooms)");
                }
            }
        }
    }
}
