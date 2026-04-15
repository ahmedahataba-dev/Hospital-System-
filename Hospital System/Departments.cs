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

        public void InitializeEmergencyDept(int floorNumber)
        {
            
            AddRoom(new Room(floorNumber * 100 + 1, Room.RoomType.ICU, floorNumber)); // Trauma
            AddRoom(new Room(floorNumber * 100 + 2, Room.RoomType.General, floorNumber)); // Exam 1
            AddRoom(new Room(floorNumber * 100 + 3, Room.RoomType.General, floorNumber)); // Exam 2
            AddRoom(new Room(floorNumber * 100 + 4, Room.RoomType.Isolation, floorNumber)); // Isolation
        }
        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }
    }
}