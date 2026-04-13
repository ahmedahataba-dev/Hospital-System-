using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Room
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public bool IsOccupied { get; set; } = false;
        public string AssignedPatient { get; set; } = "None";
        public string AssignedDoctor { get; set; } = "None";
        public int FloorNumber { get; set; }
        public DateTime LastSanitizedDate { get; set; } = DateTime.Now;
        public Room(string roomNumber, string type, int floorNumber)
        {
            RoomNumber = roomNumber;
            Type = type;
            FloorNumber = floorNumber;
        }
    }
}