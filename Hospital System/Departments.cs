using System;
using System.Collections.Generic;

namespace Hospital_System
{
    internal class Department
    {
        
        public string DeptName { get; set; }
        public List<Doctor> StaffDoctor { get; set; } = new List<Doctor>();
        public List<Nurse> StaffNurse { get; set; } = new List<Nurse>();
        public List<Room> Rooms { get; set; } = new List<Room>();

       
        public Department(string name)
        {
            DeptName = name;
        }

      
        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }
    }
}