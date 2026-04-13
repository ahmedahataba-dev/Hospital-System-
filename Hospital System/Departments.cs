using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Department
    {
        public string name { get; set; } = string.Empty;
        public string location;//floor, wing, etc.
        public List<Doctor> staffDoctor { get; set; } = new List<Doctor>();//doctors working in the department
        public List<Nurse> staffNurse { get; set; } = new List<Nurse>();//nurses working in the department
        public List<Cleaners> staffCleaner { get; set; } = new List<Cleaners>();//cleaners working in the department
        public List<Receptionists> staffReceptionist { get; set; } = new List<Receptionists>();//receptionists 
        public List<Security> staffSecurity { get; set; } = new List<Security>();//security 
        public int FloorNumber { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public void AddRoom(Room room)
        {
            if (room.FloorNumber != this.FloorNumber)
            {
                Console.WriteLine($"Error: Room {room.RoomNumber} belongs on floor {room.FloorNumber}, but {this.Name} is on floor {this.FloorNumber}!");
            }
            else
            {
                Rooms.Add(room);
                Console.WriteLine($"Room {room.RoomNumber} successfully added to {this.Name}.");
            }
        }
        public Department(string name, string location)
        {
            this.name = name;
            this.location = location;
        }
    }
}
